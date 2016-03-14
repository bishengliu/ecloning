using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class ModalSeq
    {
        //this class dispay the seqence in bootastrap3 modal
        public string Seq { get; set; }
        public int SeqCount { get; set; }
        public string aText { get; set; } //the text for the a tag
        public int idPrefix { get; set; } //could be table id
        public string Id { get; set; }
        public string Name { get; set; } //name of the seq
        public string refId { get; set; }
        public int WrapLength { get; set; }


        //construct
        public ModalSeq(string sequence, int idPrefix, string name, int WrapLength)
        {
            Seq = sequence;
            SeqCount = sequence.Length;
            this.idPrefix = idPrefix;
            this.WrapLength = WrapLength;
            Name = name;
            aText = sequence.Substring(0, 50)+"...";
        }

        public string ModalRawHTML()
        {
            Id = idPrefix.ToString() + Name;
            refId = "#" + Id;

            //sequence is divided by WrapLength
            var divide = (double)SeqCount / (double)WrapLength;
            int repeat = (int)Math.Truncate(divide);
            var seq = ecloning.Models.WordWrap.Wrap(Seq, WrapLength);

            //start with 1
            int countLeft = 1;
            int countRight = (WrapLength >= SeqCount) ? SeqCount : WrapLength;

            string html = "<span>";
            html += "<a data-toggle=\"modal\" data-target=" + refId + ">";
            //html += "<i class=\"glyphicon glyphicon-eye-open text-warning\"></i>";
            html += "<span style=\"font-family: courier !important;\">" + aText + "</span>";
            html += "</a><br/>";

            html += "<div id=\"" + Id + "\" class=\"modal fade\" role=\"dialog\">";
            html += "<div class=\"modal-dialog\">";
            html += "<div class=\"modal-content\">";
            html += "<div class=\"modal-header\">";
            html += "<button type=\"button\" class=\"close\" data-dismiss=\"modal\">&times;</button>";
            html += "<h4 class=\"modal-title text-center text-info\">" + Name + " (" + SeqCount + " bp)</h4>";
            html += "</div>";

            html += "<div class=\"modal-body\">";
            html += "<div class=\"row\">";
            html += "<div class=\"col-sm-1 text-right hidden-xs\">";
            html += "<p class=\"giraffe-seq-count\">";
            for (int i = 0; i < repeat; i++)
            {
                html += "<span>" + countLeft + "</span><br/>";
                countLeft = countLeft + WrapLength;
            }
            if (countLeft < SeqCount)
            {
                html += "<span>" + countLeft + "</span>";
            }
            html += "</p>";
            html += "</div>";

            html += "<div class=\"col-sm-10\">";
            var htmlString = new HtmlString(seq);
            html += "<p class=\"giraffe-seq-textarea\">" + htmlString + "</p>";
            html += "</div>";

            html += "<div class=\"col-sm-1 text-right hidden-xs\">";
            html += "<p class=\"giraffe-seq-count\" style=\"position: relative; left: -75px; \">";
            for (int i = 0; i < repeat; i++)
            {
                html += "<span>" + countRight + "</span><br/>";
                countRight = countRight + WrapLength;
            }
            if ((countRight - WrapLength) < SeqCount)
            {
                html += "<span>" + SeqCount + "</span>";
            }
            html += "</p>";
            html += "</div>";
            html += "</div>";
            html += "</div>";

            html += "<div class=\"modal-footer\">";
            html += "<button type=\"button\" class=\"btn btn-info\" data-dismiss=\"modal\">Close</button>";
            html += "</div>";
            html += "</div>";
            html += "</div>";
            html += "</div>";
            html += "</span>";
            return html;
        }
    }
}