define([
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/when",

    "dojo/aspect",
// epi-cms
    "epi-cms/contentediting/ContentActionSupport",
    "epi-cms/widget/_GridWidgetBase"
],

function (
    declare,
    lang,
    when,

    aspect,
    // epi-cms
    ContentActionSupport,
    _GridWidgetBase
) {

    return declare([_GridWidgetBase], {
        // summary: This component will list the latest changed pages for the web site.

        // queryName: string
        //    The name of the content query used to fetch data for the grid.
        queryName: null,

        queryParameters: null,

        dndTypes: ['epi.cms.contentreference'],

        postMixInProperties: function () {
            // summary:
            //
            // tags:
            //    protected
            this.storeKeyName = "epi.cms.content.search";

            this.inherited(arguments);
        },

        buildRendering: function () {
            // summary:
            //		Construct the UI for this widget with this.domNode initialized as a dgrid.
            // tags:
            //		protected

            this.inherited(arguments);


            var gridSettings = lang.mixin({
                columns: {
                    //name: {
                    //    renderCell: lang.hitch(this, this._renderContentItem)
                    //},
                    image: {
                        className: "epi-width35",
                        renderCell: function (item, value, node, options) {
                            node.innerHTML = "<div><img style='margin-left: auto;margin-right: auto;display: block;' src='" + item.name + "'/>" +
                                "<p style='text-align:center'>@" + item.createdBy + "</p></div>";// ContentActionSupport.getVersionStatus(value);
                
                        }
                    }
                    //,
                    //user: {
                    //    className: "epi-width15",
                    //    renderCell: function (item, value, node, options) {

                    //        console.log(item);
                    //        node.innerHTML = "@" + item.createdBy;// ContentActionSupport.getVersionStatus(value);
                
                    //    }
                    //}
            //        saved: {
            //    label: epi.resources.header.saved,
            //    formatter: this._localizeDate,
            //    className: "epi-width25"
            //}
                    
                },
                store: this.store,
                dndSourceType: this.dndTypes
            }, this.defaultGridMixin);

            this.grid = new this._gridClass(gridSettings, this.domNode);

            this.grid.set("showHeader", false);


            //this.own(this.grid.on(".dgrid-row:click", lang.hitch(this, this.onGridRowClick)));

            this.grid.on(".dgrid-cell:click", function (evt) {
                // TODO: CALL REST SERVICE AND STORE CONTENT ID + NAME (ID).
                
            }, true);

            this.own(
                aspect.around(this.grid, "insertRow", lang.hitch(this, this._aroundInsertRow))
            );
        },

        fetchData: function () {

            when(this._getCurrentItem(), lang.hitch(this, function (currentItem) {
                this.set("currentItem", currentItem);
            }));

            this.grid.set("queryOptions", { ignore: ["query"] });
            var queryParameters = this.queryParameters || {};
            queryParameters.query = this.queryName;
            this.grid.set("query", queryParameters);
        },

        _setQueryNameAttr: function (value) {
            this.queryName = value;
            this.fetchData();
        }
    });
});