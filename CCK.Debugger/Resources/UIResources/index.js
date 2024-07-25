﻿
let cckDebugger = {
    initialized: false,
    lastHoverTarget: null,
    sectionCache: {},
    buttonsCache: {},
    systemCall: function(type, arg1, arg2, arg3, arg4) {
        arg1 = arg1?.toString() || null;
        arg2 = arg2?.toString() || null;
        arg3 = arg3?.toString() || null;
        arg4 = arg4?.toString() || null;
        engine.call("CVRAppCallSystemCall", type, arg1, arg2, arg3, arg4);
    },
    playSoundCore: function(sound){
        // Possible Sounds: Hover, Click, Select, Close, Open, Warning
        cckDebugger.systemCall("PlayCoreUiSound", sound);
    },
    vibrateHand: function(delay, duration, frequency, amplitude){
        cckDebugger.systemCall("handVibrate", delay.toString(), duration.toString(), frequency.toString(), amplitude.toString());
    },
    mouseHoverButton: function(e) {
        // Find if we're hovering an element with the class cck-debugger-hover-feedback, and perform proper feedback
        let hoverTarget = null;

        if (e.target.hasAttribute("cck-debugger-feedback")) {
            hoverTarget = e.target;
        }
        else {
            let element = e.target;
            while (element.parentNode) {
                element = element.parentNode;
                if (element === document) break;
                if (element.hasAttribute("cck-debugger-feedback")) {
                    hoverTarget = element;
                    break;
                }
            }
        }

        if (hoverTarget && hoverTarget !== cckDebugger.lastHoverTarget) {
            cckDebugger.hoverFeedback(hoverTarget.getAttribute("cck-debugger-feedback"));
        }

        cckDebugger.lastHoverTarget = hoverTarget;
    },
    hoverFeedback: function(sound = "Hover") {
        cckDebugger.playSoundCore(sound);
        cckDebugger.vibrateHand(0, 0.1, 10, 1);
    },
    // Menu navigation controls
    nextMenu: () => engine.trigger('CCKDebuggerMenuNext'),
    previousMenu: () => engine.trigger('CCKDebuggerMenuPrevious'),
    nextControls: () => engine.trigger('CCKDebuggerControlsNext'),
    previousControls: () => engine.trigger('CCKDebuggerControlsPrevious'),
    // Button click events
    onButtonClick: (buttonType) => engine.call("CCKDebuggerButtonClick", buttonType),
    // Object Handlers
    onCoreInfoHandler: function(coreInfo) {
        cckDebugger.menuName.textContent = coreInfo['MenuName'];
        cckDebugger.menuNext.classList.toggle("invisible", !coreInfo['MenuEnabled'])
        cckDebugger.menuPrevious.classList.toggle("invisible", !coreInfo['MenuEnabled'])
        cckDebugger.controlsInfo.textContent = coreInfo['ControlsInfo'];
        cckDebugger.menuRoot.classList.toggle("hidden", !coreInfo['ShowSections']);
        cckDebugger.controlsPrevious.classList.toggle("invisible", !coreInfo['ShowControls']);
        cckDebugger.controlsNext.classList.toggle("invisible", !coreInfo['ShowControls']);
    },
    onButtonHandler: function(button) {
        // Properties: Type [string], IsOn [bool], IsVisible [bool]
        const buttonType = button['Type'];
        let buttonNode;
        // If the button already exists, lets just grab from the cache
        if (buttonType in cckDebugger.buttonsCache) {
            buttonNode = cckDebugger.buttonsCache[buttonType];
        }
        // Otherwise, create new button element
        else {
            buttonNode = document.createElement('div');
            buttonNode.id = `cck-debugger-button-${button['Type'].toLowerCase()}`
            buttonNode.classList.add("cck-debugger-button");
            buttonNode.setAttribute("cck-debugger-feedback", "Hover")
            buttonNode.addEventListener('click', () => cckDebugger.onButtonClick(buttonType));
            cckDebugger.buttonsRoot.appendChild(buttonNode);
            cckDebugger.buttonsCache[buttonType] = buttonNode;
        }
        // Now actually update the properties of the button
        buttonNode.classList.toggle("hidden", !button['IsVisible']);
        buttonNode.classList.toggle('on', button['IsOn']);
    },
    onSectionCreation: function(parent, section, isRoot = false) {
        // Properties: Id [int], Title [string], Value [string], Collapsable [bool], SubSections [Section[]]

        // Create the section on the parent provided
        const sectionNode = cckDebugger.onSectionHandler(parent, section, isRoot);
        const hasSubsections = section['SubSections'].length > 0;
        const isCollapsable = section['Collapsable'];

        // Add class to do the vertical margin
        if (hasSubsections) sectionNode.classList.add("cck-debugger-section-has-children");

        // Iterate the children and handle them recursively
        const subSectionNodes = []
        for (let button of section['SubSections']) {
            const subSectionNode = cckDebugger.onSectionCreation(sectionNode, button);

            if (!isCollapsable) continue;

            // Add the handlers for the collapsible stuff
            subSectionNode.classList.add("hidden");
            subSectionNodes.push(subSectionNode);
        }

        // If has children, add behavior for them
        if (hasSubsections && isCollapsable) {

            // Since it has hidden children, it's remove the hidden class from the prefix
            const { prefixClosed, prefixOpened, key } = cckDebugger.sectionCache[section['Id']];
            prefixClosed.classList.remove("hidden");
            prefixOpened.classList.add("hidden");
            key.classList.add("cck-debugger-section-key-button");
            key.setAttribute("cck-debugger-feedback", "Hover")

            // Add the click event to the section to show/hide the prefix on the section key, and hide/show the children
            const clickEvent = (event) => {
                if (scrollerComponents.wasJustDragged) return;
                prefixClosed.classList.toggle("hidden");
                prefixOpened.classList.toggle("hidden");
                sectionNode.classList.toggle('has-hidden-children');
                for (const subSectionNode of subSectionNodes) {
                    subSectionNode.classList.toggle('hidden');
                }
                event.stopPropagation();
            }
            sectionNode.addEventListener("click", clickEvent);

            // Cache the list of subsections (to be used if we have dynamic subsections)
            sectionNode['CCK.ClickEventSubSectionNodes'] = subSectionNodes;
        }

        return sectionNode;
    },
    onSectionHandler: function(parent, section, isRoot = false) {
        // Properties: Id [int], Title [string], Collapsable [bool], Value [string]
        const sectionId = section['Id'];
        let root, prefixClosed, prefixOpened, key, separator, value, sectionInfo;
        // If the section already exists, lets just grab from the cache
        if (sectionId in cckDebugger.sectionCache) {
            // Destructuring assignment to put the cache variables into values
            ({ parent, root, prefixClosed, prefixOpened, key, separator, value, sectionInfo } = cckDebugger.sectionCache[sectionId]);
        }
        // Otherwise, create new button element
        else {
            // Create the section container
            root = document.createElement('div');
            root.classList.add(isRoot ? "cck-debugger-section-root" : "cck-debugger-section")

            // Create the subsection
            sectionInfo = document.createElement('div');
            sectionInfo.classList.add("cck-debugger-section-info");
            root.appendChild(sectionInfo);

            // Create the closed prefix element part of the section
            prefixClosed = document.createElement('p');
            prefixClosed.textContent = '> ';
            // Lets have it hidden by default
            prefixClosed.classList.add("hidden");
            sectionInfo.appendChild(prefixClosed);

            // Create the opened prefix element part of the section
            prefixOpened = document.createElement('p');
            prefixOpened.textContent = 'v ';
            // Lets have it hidden by default
            prefixOpened.classList.add("hidden");
            sectionInfo.appendChild(prefixOpened);

            // Create the key/title element part of the section
            key = document.createElement('p');
            key.textContent = section['Title'];
            key.classList.add("cck-debugger-section-key");
            sectionInfo.appendChild(key);

            // Create the separator element for the key and value
            separator = document.createElement('p');
            sectionInfo.appendChild(separator);
            key.classList.add("cck-debugger-section-separator");

            // Create the value element
            value = document.createElement('p');
            value.classList.add("cck-debugger-section-value");
            sectionInfo.appendChild(value);

            // Append to the parent provided
            parent.appendChild(root);

            // Cache all elements that make a section
            cckDebugger.sectionCache[sectionId] = { parent, root, prefixClosed, prefixOpened, key, separator, value, sectionInfo };
        }
        // Now actually update the value of the section, if there is no value let's set it to empty
        value.textContent = section['Value'] ?? "";
        // If there is no value, let's ignore the separator (prettier)
        separator.textContent = section['Value'] ? ": " : "";
        // Return the container so we can use it for parenting child sections
        return root;
    }
}

engine.on('CCKDebuggerCoreUpdate', (coreJson) => {
    // console.log(coreJson);
    // return;
    // Happens when the menu structure changes
    const core = JSON.parse(coreJson);

    // Handle the core info
    cckDebugger.onCoreInfoHandler(core['Info']);

    // Clear caches since we're going to repopulate
    cckDebugger.sectionCache = {};
    cckDebugger.buttonsCache = {};

    // Clear the element children (I love frontend, very non-hacky)
    cckDebugger.menuRoot.textContent = '';
    cckDebugger.buttonsRoot.textContent = '';

    // Handle the creation of the buttons
    for (let button of core['Buttons']) {
        cckDebugger.onButtonHandler(button);
    }

    // Handle the creation of the sections recursively (using depth first)
    for (let section of core['Sections']) {
        cckDebugger.onSectionCreation(cckDebugger.menuRoot, section, true)
    }
});

engine.on('CCKDebuggerCoreInfoUpdate', (coreInfoJson) => {
    // console.log(coreInfoJson);
    // return;
    // Happens when the menu header or controls info changes
    cckDebugger.onCoreInfoHandler(JSON.parse(coreInfoJson));
});

engine.on('CCKDebuggerButtonsUpdate', (buttonsJson) => {
    // return;
    // Happens when the states of buttons change
    for (let button of JSON.parse(buttonsJson)) {
        cckDebugger.onButtonHandler(button);
    }
});

engine.on('CCKDebuggerSectionsUpdate', (sectionsJson) => {
    // return;
    // Happens when the states of sections change
    const sections = JSON.parse(sectionsJson);
    //console.log('Sections to update: ' + sections.length);
    for (let section of sections) {
        // The parent is null because at this point it should already be in the cache (I hope >.>)
        if (!section['DynamicSubsections']) {
            cckDebugger.onSectionHandler(null, section);
        }
        // Here we need to do some more work, because we have dynamic subsections ;_;
        else {
            let dynamicSection = cckDebugger.sectionCache[section['Id']]
            // Delete the sections and subsections from the cache
            delete cckDebugger.sectionCache[section['Id']]
            for (let olsSectionId of section['OldSubSectionIDs']) {
                delete cckDebugger.sectionCache[olsSectionId]
            }
            // Delete the section from the DOM
            dynamicSection.parent.removeChild(dynamicSection.root);
            // Recreate the sections and all it's sub-sections
            const replacedSection = cckDebugger.onSectionCreation(dynamicSection.parent, section)
            // Update the collapsible event of the parent
            const eventSubSectionNodes = dynamicSection.parent['CCK.ClickEventSubSectionNodes'];
            if (eventSubSectionNodes && eventSubSectionNodes.length > 0) {
                // Add the section to the event
                eventSubSectionNodes.push(replacedSection);
                // Update the hidden to match the siblings
                if (eventSubSectionNodes[0].classList.contains("hidden")) {
                    replacedSection.classList.add("hidden");
                }
                // Remove original section (no memory leak please)
                const index = eventSubSectionNodes.indexOf(dynamicSection.root);
                if (index > -1) eventSubSectionNodes.splice(index, 1);
            }
        }
    }
});

// Wait for the game to tell us it's ready
engine.on('CCKDebuggerModReady', () => {

    // Save references for later use
    cckDebugger.menuRoot = document.getElementById('cck-debugger-menu');
    cckDebugger.buttonsRoot = document.getElementById('cck-debugger-buttons');
    cckDebugger.menuName = document.getElementById('cck-debugger-menu-title');
    cckDebugger.menuNext = document.getElementById('cck-debugger-menu-next')
    cckDebugger.menuPrevious = document.getElementById('cck-debugger-menu-previous')
    cckDebugger.controlsRoot = document.getElementById('cck-debugger-controls');
    cckDebugger.controlsNext = document.getElementById('cck-debugger-controls-next');
    cckDebugger.controlsPrevious = document.getElementById('cck-debugger-controls-previous');
    cckDebugger.controlsInfo = document.getElementById('cck-debugger-controls-info');

    // Enable the hover functionality for buttons
    document.addEventListener("mousemove", cckDebugger.mouseHoverButton)

    // Setup the navigation elements
    cckDebugger.menuNext.addEventListener("click", cckDebugger.nextMenu)
    cckDebugger.menuPrevious.addEventListener("click", cckDebugger.previousMenu)
    cckDebugger.controlsNext.addEventListener("click", cckDebugger.nextControls)
    cckDebugger.controlsPrevious.addEventListener("click", cckDebugger.previousControls)

    // Mark the cohtml view as initialized
    cckDebugger.initialized = true;
    console.log("CCK Debugger is initialized and ready!")
});

// Handle scroll bars in js because cohtml funny
const scrollerComponents = {

    scrollViews: [],

    // Globals
    scrollTarget: null,

    mouseScrolling: false,
    pauseScrolling: false,

    startY: 0,
    startScrollY: 0,
    oldY: 0,
    speedY: 0,

    startX: 0,
    startScrollX: 0,
    oldX: 0,
    speedX: 0,

    scrollWheelTarget: null,

    mouseDebounceToken: null,

    wasJustDragged: false,

    initializeScrollerBars: () => {
        const scrollViews = document.querySelectorAll(".cck-debugger-scroll-view");
        for (let i = 0; i < scrollViews.length; i++) {
            // scrollerComponents.scrollViews[scrollerComponents.scrollViews.length] = new scrollerComponents.scroll_view(scrollView);
            scrollerComponents.scrollViews.push(new scrollerComponents.scroll_view(scrollViews[i]));
        }
    },

    defVal: (_val, _defVal) => {
        return (typeof(_val) !== "undefined") ? (_val !== "" ? _val : _defVal) : _defVal;
    },

    scroll_view: function(_obj) {
        this.obj = _obj;
        // this.content = cvr(_obj).find(".scroll-content").first();
        this.content = _obj.querySelector(".cck-debugger-scroll-content");
        this.viewHeight = 0;
        this.contentHeight = 0;

        const self = this;

        this.update = function() {
            self.viewHeight = scrollerComponents.defVal(self.obj.scrollHeight, 1.0);
            self.contentHeight = scrollerComponents.defVal(self.content.scrollHeight, 1.0);

            // console.log(`Heights: ${self.viewHeight} - ${self.contentHeight}`);

            const factor = self.viewHeight / self.contentHeight;
            let style = "";
            if (factor < 1.0) {
                style = "height: "+(factor*100)+"%;";
            }
            else {
                style = "height: 0;";
            }

            //console.log(`Style: ${style}`);

            const offset = self.content.scrollTop;
            const max_offset = self.contentHeight - self.viewHeight;
            const scrollPercent = offset / max_offset;
            style += "top: "+((1.0 - factor) * scrollPercent* 100)+"%;";

            // cvr(self.obj).find(".scroll-marker-v").attr("style", style);
            self.obj.querySelector(".cck-debugger-scroll-marker-v").setAttribute("style", style);
        }

        self.update();

        return {
            update: this.update
        }
    },

    initialize: () => {

        document.addEventListener('mousedown', function(e) {
            // if(e.target.hasAttribute("data-x")) {
            //     return;
            // }

            scrollerComponents.mouseDebounceToken = setTimeout(() => {

                scrollerComponents.scrollTarget = e.target.closest('.cck-debugger-scroll-content');
                scrollerComponents.startY = e.clientY;
                scrollerComponents.startX = e.clientX;
                if (scrollerComponents.scrollTarget !== null) {
                    scrollerComponents.mouseScrolling = true;
                    scrollerComponents.startScrollY = scrollerComponents.scrollTarget.scrollTop;
                    scrollerComponents.startScrollX = scrollerComponents.scrollTarget.scrollLeft;
                }

                scrollerComponents.mouseDebounceToken = null;

            }, 200);

        });

        document.addEventListener('mousemove', function(e) {
            if (scrollerComponents.scrollTarget !== null && scrollerComponents.mouseScrolling && !scrollerComponents.pauseScrolling) {
                scrollerComponents.scrollTarget.scrollTop = scrollerComponents.startScrollY - e.clientY + scrollerComponents.startY;
                scrollerComponents.scrollTarget.scrollLeft = scrollerComponents.startScrollX - e.clientX + scrollerComponents.startX;
                scrollerComponents.speedY = e.clientY - scrollerComponents.oldY;
                scrollerComponents.speedX = e.clientX - scrollerComponents.oldX;
                scrollerComponents.oldY = e.clientY;
                scrollerComponents.oldX = e.clientX;
                //console.log(`Mouse Move: ${scrollerComponents.scrollTarget.scrollTop} => ${scrollerComponents.startScrollY} - ${e.clientY} + ${scrollerComponents.startY} = ${scrollerComponents.startScrollY - e.clientY + scrollerComponents.startY}`);
                scrollerComponents.UpdateScrollViews();
            }

            scrollerComponents.scrollWheelTarget = e.target.closest('.cck-debugger-scroll-content');
        });

        document.addEventListener('mouseup', function(e) {

            if (scrollerComponents.mouseDebounceToken === null) {

                scrollerComponents.mouseScrolling = false;
                if (scrollerComponents.scrollTarget != null) {
                    scrollerComponents.startScrollY = scrollerComponents.scrollTarget.scrollTop;
                    scrollerComponents.startScrollX = scrollerComponents.scrollTarget.scrollLeft;
                }

                scrollerComponents.wasJustDragged = true;
                setTimeout(() => scrollerComponents.wasJustDragged = false, 10);

            }
            else {
                clearTimeout(scrollerComponents.mouseDebounceToken);
                scrollerComponents.mouseDebounceToken = null;
            }

            // Update the scroll bar when stuff gets collapsed/expanded
            setTimeout(() => scrollerComponents.UpdateScrollViews(), 10);

        });

        window.setInterval(function() {
            if (!scrollerComponents.mouseScrolling && scrollerComponents.scrollTarget != null && (Math.abs(scrollerComponents.speedY) > 0.01 || Math.abs(scrollerComponents.speedX) > 0.01) && !scrollerComponents.pauseScrolling) {
                scrollerComponents.speedY *= 0.95;
                scrollerComponents.speedX *= 0.95;

                scrollerComponents.scrollTarget.scrollTop -= scrollerComponents.speedY;
                scrollerComponents.scrollTarget.scrollLeft -= scrollerComponents.speedX;
                scrollerComponents.UpdateScrollViews();
            }
            else if (!scrollerComponents.mouseScrolling && scrollerComponents.scrollTarget != null) {
                scrollerComponents.scrollTarget = null;
            }
        }, 10);

        window.addEventListener('wheel', function(e) {
            if (scrollerComponents.scrollWheelTarget != null) {
                scrollerComponents.scrollWheelTarget.scrollTop += e.deltaY;
                scrollerComponents.scrollWheelTarget.scrollLeft += e.deltaX;
                scrollerComponents.UpdateScrollViews();
            }
        });

        scrollerComponents.UpdateScrollViews = () => {
            for (const scrollerComponent of scrollerComponents.scrollViews) {
                scrollerComponent.update();
            }
        }

        scrollerComponents.initializeScrollerBars();
    }
}

scrollerComponents.initialize();

// Tell the game we're ready (needs to be executed at the end of the file).
engine.trigger('CCKDebuggerMenuReady');
