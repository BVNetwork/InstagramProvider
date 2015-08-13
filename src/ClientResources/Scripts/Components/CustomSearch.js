define([
    "dojo/_base/declare",
    "dojo/dom-geometry",
    "dijit/_TemplatedMixin",
    "dijit/_Container",
    "dijit/layout/_LayoutWidget",
    "dijit/_WidgetsInTemplateMixin",

    //"epi-cms/dgrid/DnD",
    "permagne/components/CustomGrid",
    "dojo/text!./templates/CustomSearch.html"],
    function (// Dojo        
        declare,
        domGeometry,
        // Dijit    
        _TemplatedMixin,
        _Container,
        _LayoutWidget,
        _WidgetsInTemplateMixin,
        // EPi CMS    
        ContentQueryGrid,
        template) {
        return declare([
            _Container,
            _LayoutWidget,
            _TemplatedMixin,
            _WidgetsInTemplateMixin],

            {   // summary: This component enabled searching of content where the results will be displayed in a grid.         
                templateString: template,

                resize: function (newSize) {
               
                    this.inherited(arguments);
                
                    var toolbarSize = domGeometry.getMarginBox(this.toolbar);
                   
                    var gridSize = { w: newSize.w, h: 1000 };
                    this.contentQuery.resize(gridSize);

                },

                _reloadQuery: function () {
                    this.contentQuery.set("queryParameters", { queryText: this.queryText.value });
                    this.contentQuery.set("queryName", "InstagramQuery");

                }
            });
    });