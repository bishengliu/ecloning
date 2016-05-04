using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class pBundle
    {
        public string Name { get; set; }
        public int bundle_id { get; set; }
        public string Des { get; set; }
        public string Upload { get; set; }

        public int parentalBundle { get; set; } //this will be created from parant bundle
        public List<BundleItem> Plasmids { get; set; }

    }

    public class BundleItem
    {
        public int plasmidId { get; set; }
        public string plasmidRole { get; set; }
    }

    public class BundleHTML
    {
        public string outPut(int count, int id, string modalRefId, string mapId, string idDiv, string modalId, string modalMapId)
        {
            var sb = new System.Text.StringBuilder();
            //parent div
            sb.Append("<div class=\"col-xs-12 col-sm-6 col-md-4 col-lg-4\">");
                //map wrapped inside a
                sb.Append("<a href=\"#\" data-toggle=\"modal\" data-target="+ modalRefId +" class=\"pull-left\">");
                    //map
                    sb.Append("<div class=\"col-xs-12\" id="+mapId+"></div>");
                sb.Append("</a>");

                //trash and modal
                sb.Append("<div class=\"col-xs-12\">");
                    //trash
                    sb.Append("<a href=\"#\" class=\"pull-right delete\" id=\""+ idDiv +"\">");
                        sb.Append("<i class=\"fa fa-2x fa-trash-o text-warning\"></i>");
                        sb.Append("<span class=\"text-warning\"></span>");
                    sb.Append("</a>");

                    //modal
                    sb.Append("<div id="+modalId+" class=\"modal fade\" role=\"dialog\">");
                        sb.Append("<div class=\"modal-dialog\">");
                            sb.Append("<div class=\"modal-content\">");
                                sb.Append("<div class=\"modal-header\">");
                                    sb.Append("<button type=\"button\" class=\"close\" data-dismiss=\"modal\">&times;</button>");
                                    sb.Append("<h4 class=\"modal-title text-center\">Plasmid Map</h4>");
                                sb.Append("</div>");
                                sb.Append("<div class=\"modal-body\">");
                                    sb.Append("<div id="+modalMapId+"></div>");
                                sb.Append("</div>");
                                sb.Append("<div class=\"modal-footer\">");
                                    sb.Append("<button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">Close</button>");
                                sb.Append("</div>");
                            sb.Append("</div>");
                        sb.Append("</div>");
                    sb.Append("</div>");

                sb.Append("</div>");

                //form data
                sb.Append("<div class=\"col-xs-12\">");
                    //hidden id
                    sb.Append("<div class=\"hidden\">");
                    sb.Append("@Html.LabelFor(model => model.Plasmids["+count+"].plasmidId, \"Plasmid Id\", htmlAttributes: new { @class = \"control-label\" })");
                    sb.Append("@Html.EditorFor(model => model.Plasmids[" + count + "].plasmidId, new { htmlAttributes = new { @class = \"form-control\", @Value = "+ id +" } })");            
                    sb.Append("</div>");

                    //comment
                    sb.Append("@Html.LabelFor(model => model.Plasmids[" + count + "].plasmidRole, \"Comment\", htmlAttributes: new { @class = \"control-label\" })");
                    sb.Append("@Html.TextAreaFor(model => model.Plasmids[" + count + "].plasmidRole, 4, 150, new { @class = \"form-control\" })");
                    sb.Append("@Html.ValidationMessageFor(model => model.Plasmids[" + count + "].plasmidRole, \"\", new { @class = \"text-danger\" })");

                sb.Append("</div>");

            sb.Append("</div>");

            return sb.ToString();
        }
    }

}