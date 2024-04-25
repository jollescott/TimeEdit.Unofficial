globalThis.document = {
    createElement: function () {
        
    }
}

globalThis.window = {
    document: document,
    window: this
};

globalThis.module = {
    exports: {}
};