﻿//button events of seq-editor
$("#delete_seq").on("click", function (e) {
    var seq = $('#seq').text().trim();
    var seqArray = seq.split('');
    var seq_len = seqArray.length;
    $("#delete_msg").text(null);
    e.preventDefault();
    if (!$('#from_pos').val().trim() || !$('#to_pos').val().trim()) {
        $("#delete_msg").text("Both positions are required!");
        return false;
    }
    if (+$('#from_pos').val().trim() < 1 || +$('#to_pos').val().trim() > seq_len) {
        $("#delete_msg").text("Invalid position!");
        return false;
    }
    //update svg

    var from = +$('#from_pos').val().trim();
    var to = +$('#to_pos').val().trim();

    for (i = from; i <= to; i++) {
        var curId = "#" + i + "-letter";
        $(curId).addClass("removed").css("text-decoration", "line-through")
                              .css("font-size", "15px")
                              .css("stroke", "#000000");
        //update EditObj
        EditObj = genEditObj(i, seqArray[i - 1], "delete", null, null, EditObj);
    }
    //console.log(EditObj);
});//delete letters
//format sequence
$('#insert_seq').change(function () {
    //type
    var type = $("#type").text().trim();
    var before = $('#insert_seq').val();
    //strip out non-alpha characters and convert to uppercase
    var after = before.replace(/[^a-zA-Z]+|\s+$|[0-9]+/g, '').toUpperCase();
    if (type == "DNA") {
        after = after.replace(/[bdefhijklmnopqrsuvwxyzBDEFHIJKLMNOPQRSUVWZYX]+|\s+$|[0-9]+/g, '').toUpperCase();
    }
    else {
        //RNA
        after = after.replace(/[bdefhijklmnopqrstvwxyzBDEFHIJKLMNOPQRSTVWZYX]+|\s+$|[0-9]+/g, '').toUpperCase();
    }
    $('#insert_seq').val(after);
});
$("#insert").on("click", function (e) {
    var seq = $('#seq').text().trim();
    var seqArray = seq.split('');
    var seq_len = seqArray.length;
    var r = 10;
    var type = $("#type").text().trim();
    if (type == "DNA") {
        var color = d3.scale.ordinal()
              .domain(["A", "T", "G", "C"])
              .range(["#3366cc", "#dc3912", "#ff9900", "#109618"]);
    } else {
        //RNA
        var color = d3.scale.ordinal()
              .domain(["A", "U", "G", "C"])
              .range(["#3366cc", "#dc3912", "#ff9900", "#109618"]);
    }
    //select option value
    var aob = $("#aob").val();

    $("#insert_msg").text(null);
    e.preventDefault();
    if (!$('#insert_pos').val().trim()) {
        $("#insert_msg").text("position is required!");
        return false;
    }
    if (+$('#insert_pos').val().trim() < 1 || +$('#insert_pos').val().trim() > seq_len) {
        $("#insert_msg").text("Invalid position!");
        return false;
    }

    if (!$('#insert_seq').val().trim()) {
        $("#insert_msg").text("Sequence is required!");
        return false;
    }
    //get leftId
    var posString = null;
    var leftId = "-insert";
    var divId = "-div";
    if (aob == "After") {
        posString = $('#insert_pos').val().trim() + "-" + (+$('#insert_pos').val().trim() + 1);
        leftId = posString + leftId;
        divId = posString + divId;
    }
    else {
        posString = (+$('#insert_pos').val().trim() - 1) + "-" + $('#insert_pos').val().trim();
        leftId = posString + leftId;
        divId = posString + divId;
    }
    var curPos = +$('#insert_pos').val().trim();
    //get the x, y position in the SeqObj
    curObj = $.grep(seqObj, function (d) {
        return (+d[1] === +curPos);
    });
    var curLetter = curObj[0][0];
    var xPos = curObj[0][2];
    var yPos = curObj[0][3];
    var nLetters = $('#insert_seq').val().trim();

    //update svg
    if (aob == "Before") {
        if ($("#" + leftId).length == 0) {
            //there is no insert here yet
            var leftgroup = d3.select("svg").append("g")
                                        .attr("id", leftId)
                                        .attr("class", "clickable")
                                        .on("dblclick", function () {
                                            //delete this
                                            d3.select(this).remove();
                                            //hide seq-tooltip
                                            $("#" + divId).css("opacity", 0);
                                            //delete data object
                                            removeEditObj("insert", leftId, EditObj);
                                        });
            var leftSlash = leftgroup.append("line")
                                             .style("stroke", "gray")
                                             .attr("x1", xPos + 30)
                                             .attr("y1", yPos + 45)
                                             .attr("x2", xPos + 20)
                                             .attr("y2", yPos + 30);
            //apend letter

            var leftLetter = leftgroup.append("text").attr("class", "leftLetter letter clickable")
                                                    .attr("y", yPos + 40)
                                                    .attr("x", xPos + 27)
                                                    .attr("id", posString + "-letter")
                                                    .attr("font-size", "15px")
                                                    .attr("font-weight", "bold")
                                                    .attr("stroke", "#756bb1")
                                                    .style("fill", function () { return color(nLetters); })
                                                    .style("opacity", nLetters.length == 1 ? 0.9 : 0)
                                                    .text(nLetters);

            //append line
            var lineId = posString + "-line";
            var leftline = leftgroup.append("line").attr("id", lineId).attr("class", "clickable")
                                       .style("stroke", "purple")
                                       .style("stroke-width", 2)
                                       .attr("x1", xPos + 27)
                                       .attr("y1", yPos + 35)
                                       .attr("x2", xPos + 37)
                                       .attr("y2", yPos + 35)
                                       .attr("stroke-dasharray", "2, 2, 2, 2")
                                       .style("opacity", nLetters.length == 1 ? 0 : 0.9);
            //append a div in body
            var insertDiv = d3.select("body").append("div")
                              .attr("id", divId)
                              .attr("class", "seq-tooltip")
                              .style("opacity", 0);
            $("#" + divId).html("<span class=\"text-info\">" + nLetters + "</span>");
            //dot line event
            leftline.on("mouseover", function () {
                insertDiv.transition()
                 .duration(200)
                 .style("opacity", .9)
                 .style("left", (d3.event.pageX - 40) + "px")
                  .style("top", (d3.event.pageY - 50) + "px");
            })
                    .on("mouseleave", function () {
                        insertDiv.transition()
                            .duration(200)
                            .style("opacity", 0);
                    });
            //update EditObj
            EditObj = genEditObj(curPos, curLetter, "insert", "left", nLetters, EditObj);
            //genEditObj(pos, oriLetter, action, lor, newLetter, EditObj)

        }
        else {
            //check whether it is letter for a plus symbol
            if ($("#" + leftId).length !== 0) {
                //this is a letter
                //get the nletter
                var previous_letter = $("#" + posString + "-letter").text();
                var nletters = previous_letter + nLetters;
                //find the div
                //save the letter in the new div
                $("#" + divId).html(function (d) { return "<span class=\"text-info\">" + nletters + "</span>"; });

                //remove leftLetter
                $("#" + posString + "-letter").remove();
                //show dash line
                var lineId = posString + "-line";
                $("#" + lineId).css("opacity", .9);
                //update EditObj
                EditObj = genEditObj(curPos, curLetter, "insert", "left", nletters, EditObj);
            }
            else {
                //this is a plus div
                var divId = posString + "-div";
                var previous_letter = $("#" + divId + " span").text();
                var nletters = previous_letter + nLetters;
                $("#" + divId).empty();
                $("#" + divId).html(function (d) { return "<span class=\"text-info\">" + nletters + "</span>"; });
                //update EditObj
                EditObj = genEditObj(curPos, curLetter, "insert", "left", nletters, EditObj);
            }
        }
    }
    else {
        var xshift = +2 * r;
        if ($("#" + leftId).length === 0) {
            var leftgroup = d3.select("svg").append("g")
                                .attr("id", leftId)
                                .attr("class", "clickable")
                                .on("dblclick", function () {
                                    //delete this
                                    d3.select(this).remove();
                                    //hide seq-tooltip
                                    $("#" + divId).css("opacity", 0);
                                    //delete data object
                                    removeEditObj("insert", leftId, EditObj);
                                });
            var leftSlash = leftgroup.append("line")
                                     .style("stroke", "gray")
                                     .attr("x1", xPos + 30 + xshift)
                                     .attr("y1", yPos + 45)
                                     .attr("x2", xPos + 20 + xshift)
                                     .attr("y2", yPos + 30);
            //apend letter

            var leftLetter = leftgroup.append("text").attr("class", "leftLetter letter clickable")
                                                        .attr("y", yPos + 40)
                                                        .attr("x", xPos + 27 + xshift)
                                                        .attr("id", posString + "-letter")
                                                        .attr("font-size", "15px")
                                                        .attr("font-weight", "bold")
                                                        .attr("stroke", "#756bb1")
                                                        .style("fill", function () { return color(nLetters); })
                                                        .style("opacity", nLetters.length == 1 ? 0.9 : 0)
                                                        .text(nLetters);

            //append line
            var lineId = posString + "-line";
            var leftline = leftgroup.append("line").attr("id", lineId).attr("class", "clickable")
                                       .style("stroke", "purple")
                                       .style("stroke-width", 2)
                                       .attr("x1", xPos + 27 + xshift)
                                       .attr("y1", yPos + 35)
                                       .attr("x2", xPos + 37 + xshift)
                                       .attr("y2", yPos + 35)
                                       .attr("stroke-dasharray", "2, 2, 2, 2")
                                       .style("opacity", nLetters.length == 1 ? 0 : 0.9);
            //append a div in body
            var divId = posString + "-div";
            var insertDiv = d3.select("body").append("div")
                              .attr("id", divId)
                              .attr("class", "seq-tooltip")
                              .style("opacity", 0);
            $("#" + divId).html("<span class=\"text-info\">" + nLetters + "</span>");

            //dot line event
            leftline.on("mouseover", function () {
                insertDiv.transition()
                 .duration(200)
                 .style("opacity", .9)
                 .style("left", (d3.event.pageX - 40) + "px")
                  .style("top", (d3.event.pageY - 50) + "px");
            })
                    .on("mouseleave", function () {
                        insertDiv.transition()
                            .duration(200)
                            .style("opacity", 0);
                    });
            //update EditObj
            if (curPos == seq_len) {
                //length + 1************************************************************************
                EditObj = genEditObj(curPos + 2, curLetter, "insert", "left", nLetters, EditObj);
            }
            else {
                EditObj = genEditObj(curPos + 1, curLetter, "insert", "left", nLetters, EditObj);
            }
            //genEditObj(pos, oriLetter, action, lor, newLetter, EditObj)

        }
        else {
            //check whether it is letter for a plus symbol
            if ($("#" + posString + "-letter").length !== 0) {
                //this is a letter
                //get the nletter
                var previous_letter = $("#" + posString + "-letter").text();
                var nletters = nLetters + previous_letter;
                //find the div
                //save the letter in the new div
                $("#" + divId).html(function (d) { return "<span class=\"text-info\">" + nletters + "</span>"; });

                //remove leftLetter
                $("#" + posString + "-letter").remove();
                //show dash line
                var lineId = posString + "-line";
                $("#" + lineId).css("opacity", .9);
                //update EditObj
                if (curPos == seq_len) {
                    //length + 1************************************************************************
                    EditObj = genEditObj(curPos + 2, curLetter, "insert", "left", nletters, EditObj);
                }
                else {
                    EditObj = genEditObj(curPos + 1, curLetter, "insert", "left", nletters, EditObj);
                }
            }
            else {
                //this is a plus div
                var previous_letter = $("#" + divId + " span").text();
                var nletters = nLetters + previous_letter;
                $("#" + divId).empty();
                $("#" + divId).html(function (d) { return "<span class=\"text-info\">" + nletters + "</span>"; });
                //update EditObj
                if (curPos == seq_len) {
                    //length + 1************************************************************************
                    EditObj = genEditObj(curPos + 2, curLetter, "insert", "left", nletters, EditObj);
                }
                else {
                    EditObj = genEditObj(curPos + 1, curLetter, "insert", "left", nletters, EditObj);
                }
            }
        }
    }
});//insert letters

function validateForm() {
    //validate the form submission
    var oldSeq = $('#old_seq').val();
    if (!oldSeq.trim()) {
        $('#error').text("\"Sequence\" is required!");
        return false;
    }
    return true;
};
function seqEditor(seq, id, type) {
    //convert the seq to arary
    var seqArray = seq.split('');
    var seq_len = seqArray.length;

    //prepare svg
    var margin = { top: 50, right: 40, bottom: 60, left: 40 },
        width = ($(id).width() <= 250 ? 250 : $(id).width()) - margin.left - margin.right;
    var padding = 1.0; //half of the step
    var r = 10; //radius of the circle
    var pr = 1; //point
    //using d3 rangePoints
    var step = 2 * r; //distance between 2 necleotides
    var numCol = Math.floor(width / step); //number of nucleotides per line
    var arrCol = genArray(numCol);
    var numRow = Math.round(seq_len / numCol) < 2 ? 2 : Math.round(seq_len / numCol);
    var arrRow = genArray(numRow);
    //height of svg
    var height = numRow * (step * 3); //step in vertical is half of the horizontal
    var x = d3.scale.ordinal()
          .domain(arrCol)
          .rangePoints([1, width], padding);
    var y = d3.scale.ordinal()
              .domain(arrRow)
              .rangePoints([1, height], padding);
    var xRange = x.range()[1] - x.range()[0];
    var yRange = y.range()[1] - y.range()[0];
    //process the seq data
    seqObj = genSeqObj(seqArray, seq_len, numCol, numRow, xRange, yRange);
    if (type == "DNA") {
        var color = d3.scale.ordinal()
              .domain(["A", "T", "G", "C"])
              .range(["#3366cc", "#dc3912", "#ff9900", "#109618"]);
    } else {
        //RNA
        var color = d3.scale.ordinal()
              .domain(["A", "U", "G", "C"])
              .range(["#3366cc", "#dc3912", "#ff9900", "#109618"]);
    }
    var xAxis = d3.svg.axis()
            .scale(x)
            .innerTickSize(-height).outerTickSize(0).tickPadding(10)
            .orient("bottom");
    var yAxis = d3.svg.axis()
                .scale(y)
                .innerTickSize(-width).outerTickSize(0).tickPadding(10)
                .tickFormat(function (d) { return d == 1 ? d : (d - 1) * numCol + 1; })
                .orient("left");
    var svg = d3.select(id).append("svg")
                .attr("width", width + margin.left + margin.right)
                .attr("height", height + margin.top + margin.bottom)
              .append("g")
                .attr("transform", "translate(" + margin.left + "," + margin.top + ")");
    //draw points
    var g = svg.append("g").attr("class", "unit");

    var points = g.selectAll(".point")
                     .data(seqObj)
                  .enter()
                     .append("circle");
    var pointsAttributes = points
                            .attr("class", "point")
                            .attr("id", function (d) { return d[1] + "-point" })
                            .attr("cx", function (d) { return d[2]; })
                            .attr("cy", function (d) { return d[3] + 25; })
                            .attr("r", pr)
                            .attr("display", function (d) { return d[1] % 5 === 0 ? "none" : "inline "; })
                            //.style("stroke", "none")
                            //.style("stroke", function (d) { return color(d[0]); })
                            //.style("stroke-width", 0.3)
                            .style("fill-opacity", 0.2)
                            .style("fill", "#3b3eac");
    //.style("fill", function (d) { return color(d[0]); });

    //add seq count
    var counts = g.selectAll(".count")
                .data(seqObj)
             .enter()
                .append("text")
                                .attr({
                                    "alignment-baseline": "middle",
                                    "text-anchor": "middle"
                                })
                                .attr("y", function (d) { return d[3] + 25; })
                                .attr("x", function (d) { return d[2]; })
                                .attr("id", function (d) { return d[1] + "-count" })
                                .attr("class", "count")
                                .attr("class", "noselect")
                                .attr("display", function (d) { return d[1] % 5 !== 0 ? "none" : "inline "; })
                                .attr("font-size", "10px")
                                .attr("font-family", "monospace")
                                .style("fill-opacity", 0.2)
                                .style("fill", "#3b3eac")
                                .text(function (d) { return d[1] + "" });
    //add neucleotide letters
    var text = g.selectAll(".letter")
                .data(seqObj)
             .enter()
                .append("text")
                                .attr({
                                    "alignment-baseline": "middle",
                                    "text-anchor": "middle"
                                })
                               .attr("y", function (d) { return d[3]; })
                                .attr("x", function (d) { return d[2]; })
                                .attr("class", "letter")
                                //.attr("class", "noselect")
                                .attr("class", "clickable")
                                .attr("id", function (d) { return d[1] + "-letter" })
                                .style("fill", function (d) { return color(d[0]); })
                                .text(function (d) { return d[0] + "" });
    //add events on letter
    text.on("mouseover", function (d) {
        //get data
        var target = d3.select(this);
        var target_data = target[0][0].__data__;
        var curLetter = target_data[0];
        var curPos = target_data[1];
        var curId = "#" + curPos + "-letter";

        //preparep tooltip div
        var tooltip = d3.select("body").append("div")
                            .attr("class", "seq-tool")
                            .style("opacity", 0);
        //============points and count===================
        //point
        var pointId = "#" + d[1] + "-point";
        var point = $(pointId);
        //count
        var countId = "#" + d[1] + "-count";
        var count = $(countId);
        if (d[1] % 5 === 0) {
            //point is original hidden, do nothing on point
            //test is original show, change the text color
            count.css({ "fill-opacity": 0.5 });
            count.css({ fill: "red" });

        }
        else {
            //point is originaly shown, hide it
            point.css({ display: "none" });
            //count is originally hiden, now show it
            count.css({ display: "inline" });

            //change color
            count.css({ "fill-opacity": 0.5 });
            count.css({ fill: "red" });
        }
        /*
        //hide current letter
        target.attr("display", "none");
        */
        //===========tool======================
        var tx = d[2] + 4 * r;
        var ty = d[3] + 2 * r + 29;
        //data to define the position of buttons
        if (type == "DNA") {
            var dataLetter = [

                            { info: "<span class=\"text-info\">Insert \"A\"</span><br/><span class=\"tooltip-smallfont text-info\">After Current</span>", type: "after", letter: "A", value: 0.5 },
                            { info: "<span class=\"text-info\">Insert \"T\"</span><br/><span class=\"tooltip-smallfont text-info\">After Current</span>", type: "after", letter: "T", value: 0.5 },
                            { info: "<span class=\"text-info\">Insert \"G\"</span><br/><span class=\"tooltip-smallfont text-info\">After Current</span>", type: "after", letter: "G", value: 0.5 },
                            { info: "<span class=\"text-info\">Insert \"C\"</span><br/><span class=\"tooltip-smallfont text-info\">After Current</span>", type: "after", letter: "C", value: 0.5 },

                            { info: "<span class=\"text-info\">Change to \"A\"</span>", type: "change", letter: "A", value: 0.5 },
                            { info: "<span class=\"text-info\">Change to \"T\"</span>", type: "change", letter: "T", value: 0.5 },
                            { info: "<span class=\"text-info\">Change to \"G\"</span>", type: "change", letter: "G", value: 0.5 },
                            { info: "<span class=\"text-info\">Change to \"C\"</span>", type: "change", letter: "C", value: 0.5 },

                            { info: "<span class=\"text-info\">Insert \"A\"</span><br/><span class=\"tooltip-smallfont text-info\">Before Current</span>", type: "before", letter: "A", value: 0.5 },
                            { info: "<span class=\"text-info\">Insert \"T\"</span><br/><span class=\"tooltip-smallfont text-info\">Before Current</span>", type: "before", letter: "T", value: 0.5 },
                            { info: "<span class=\"text-info\">Insert \"G\"</span><br/><span class=\"tooltip-smallfont text-info\">Before Current</span>", type: "before", letter: "G", value: 0.5 },
                            { info: "<span class=\"text-info\">Insert \"C\"</span><br/><span class=\"tooltip-smallfont text-info\">Before Current</span>", type: "before", letter: "C", value: 0.5 }
            ];
        }
        else {
            //RNA
            var dataLetter = [

            { info: "<span class=\"text-info\">Insert \"A\"</span><br/><span class=\"tooltip-smallfont text-info\">After Current</span>", type: "after", letter: "A", value: 0.5 },
            { info: "<span class=\"text-info\">Insert \"T\"</span><br/><span class=\"tooltip-smallfont text-info\">After Current</span>", type: "after", letter: "U", value: 0.5 },
            { info: "<span class=\"text-info\">Insert \"G\"</span><br/><span class=\"tooltip-smallfont text-info\">After Current</span>", type: "after", letter: "G", value: 0.5 },
            { info: "<span class=\"text-info\">Insert \"C\"</span><br/><span class=\"tooltip-smallfont text-info\">After Current</span>", type: "after", letter: "C", value: 0.5 },

            { info: "<span class=\"text-info\">Change to \"A\"</span>", type: "change", letter: "A", value: 0.5 },
            { info: "<span class=\"text-info\">Change to \"T\"</span>", type: "change", letter: "U", value: 0.5 },
            { info: "<span class=\"text-info\">Change to \"G\"</span>", type: "change", letter: "G", value: 0.5 },
            { info: "<span class=\"text-info\">Change to \"C\"</span>", type: "change", letter: "C", value: 0.5 },

            { info: "<span class=\"text-info\">Insert \"A\"</span><br/><span class=\"tooltip-smallfont text-info\">Before Current</span>", type: "before", letter: "A", value: 0.5 },
            { info: "<span class=\"text-info\">Insert \"T\"</span><br/><span class=\"tooltip-smallfont text-info\">Before Current</span>", type: "before", letter: "U", value: 0.5 },
            { info: "<span class=\"text-info\">Insert \"G\"</span><br/><span class=\"tooltip-smallfont text-info\">Before Current</span>", type: "before", letter: "G", value: 0.5 },
            { info: "<span class=\"text-info\">Insert \"C\"</span><br/><span class=\"tooltip-smallfont text-info\">Before Current</span>", type: "before", letter: "C", value: 0.5 }
            ];
        }

        var toolcolor = d3.scale.ordinal()
            .domain(["change", "before", "after"])
            .range(["#0099c6", "#994499", "#66aa00"]);
        var toolpie = d3.layout.pie()
            .value(function (d) { return d.value; })
            .sort(null);

        var arc = d3.svg.arc()
            .outerRadius(5 * r - 10);

        //var pie = d3.layout.pie(function (d) { return d.v; });

        var tool = d3.select("svg").append("g").attr("class", "tool")
                     .on("mouseleave", function (d) {
                         //hide tool
                         d3.selectAll(".tool")
                             .transition().delay(150)//.attr("transform", "scale(0)");
                             .attr("display", "none");
                         //delete all tooltip
                         d3.selectAll(".tooltip2").remove();
                     })
                     .datum(dataLetter).attr("transform", "translate(" + tx + "," + ty + ")");

        //big toolcircle
        var toolcircle = tool.selectAll("path")
                      .data(toolpie)
                       .enter();

        var toolpie = toolcircle.append("path").attr("class", "toolcircle")
                        .style("fill-opacity", 0.8)
                        .attr("class", "clickable")
                        .style("fill", function (d, i) { return toolcolor(d.data.type); })
                        .on("mouseover", function (d) {
                            //here to add events
                            var tgt = d3.select(this);
                            var tgtdata = tgt[0][0].__data__.data;
                            tgt.style("fill-opacity", 1.0);
                            //show tooltip
                            tooltip.transition()
                                    .duration(200)
                                    .style("opacity", .9);
                            tooltip.html(tgtdata.info)
                                .style("left", (d3.event.pageX + 10) + "px")
                                .style("top", (d3.event.pageY + 30) + "px");
                        })
                        .on("mouseleave", function (d) {
                            var tgt = d3.select(this);
                            tgt.style("fill-opacity", 0.8);
                            //hide tooptip
                            tooltip.transition()
                                .duration(500)
                                .style("opacity", 0);
                        })
                        .on("click", function (d) {
                            //find which pie is the current tgt        
                            var ctgt = d3.select(this);
                            toolPieClick(d, ctgt, target_data, color, r, seq_len);
                        });
        //show the big circle
        toolpie
            .transition()
            .duration(1000)
            .attrTween("d", function (d, i, a) {
                var i = d3.interpolate(this._current, d.data.endAngle);
                this._current = i(0);
                return function (t) {
                    return arc(d);
                };
            });

        //add "A", "T","G" and C on the toolcircle
        var toolletter = toolcircle.append("text")
                                   .attr("dy", ".35em")
                                   .attr("class", "clickable")
                                   .attr("text-anchor", "middle")
                                   .attr("transform", function (d) {
                                       d.innerRadius = 5 * r / 2 - 10; // Set Inner Coordinate
                                       return "translate(" + arc.centroid(d) + ")rotate(" + angle(d) + ")";
                                   })
                                    .style("fill", "White")
                                    .style("font", "10px Arial")
                                    .style("font-weight", "bold")
                                    .text(function (d) { return d.data.letter; });
        //tool letter events
        toolletter.on("click", function (d) {
            //find which pie is the current tgt        
            var ctgt = d3.select(this);
            toolPieClick(d, ctgt, target_data, color, r, seq_len);
        });

        //add inner criclefor remove function
        var innerarc = d3.svg.arc()
            .outerRadius(5 * r / 2 - 10)
            .startAngle(0)
            .endAngle(Math.PI * 2);
        var innerCircle = tool.append("path").attr("class", "innercircle")
                        .style("fill-opacity", 0.6)
                        .attr("class", "clickable")
                        .style("fill", "#fd8d3c")
                        .attr("d", innerarc)
                        .on("mouseover", function () {
                            var tgt = d3.select(this);
                            tgt.style("fill-opacity", 1.0);
                            //show tooltip
                            tooltip.transition()
                                    .duration(200)
                                    .style("opacity", .9);
                            tooltip.html(function (d) { return "<span class=\"text-danger\"><i class=\"fa fa-trash-o text-danger\"></i> Remove</span>"; })
                                .style("left", (d3.event.pageX - 40) + "px")
                                .style("top", (d3.event.pageY + 50) + "px");
                        })
                        .on("mouseleave", function (d) {
                            var tgt = d3.select(this);
                            tgt.style("fill-opacity", 0.6);
                            //hide tooptip$
                            tooltip.transition()
                                .duration(500)
                                .style("opacity", 0);
                        });
        //add text with strike-through in the remove-inner circle
        var removeLetter = tool.append("text")
                              .attr("dy", ".35em")
                              .attr("text-anchor", "middle")
                              .attr("class", "clickable")
                              .style("fill", "white")
                              .style("font", "13px Arial")
                              .style("font-weight", "bold")
                              .style("text-decoration", "line-through")
                              .style("strikethrough-thickness", 4)
                              .style("stroke", "red")
                              .text(curLetter);
        //innercircle click event
        innerCircle.on("click", function (d) {
            //hide tool
            d3.selectAll(".tool")
              .transition().delay(150)
              .attr("display", "none");
            //add strikethrough on the seq neceotide and add removed class
            $(curId).addClass("removed").css("text-decoration", "line-through")
                              //.css("text-decoration-color", "red")
                              .css("font-size", "15px")
                              .css("stroke", "#000000");
            //update EditObj
            EditObj = genEditObj(curPos, curLetter, "delete", null, null, EditObj);
        });
        //innnerLetter click event
        removeLetter.on("click", function (d) {
            //hide tool
            d3.selectAll(".tool")
              .transition().delay(150)
              .attr("display", "none");
            //add strikethrough on the seq neceotide
            $(curId).addClass("removed").css("text-decoration", "line-through")
                              //.css("text-decoration-color", "red")
                              .css("font-size", "15px")
                              .css("stroke", "#000000");
            //update EditObj
            EditObj = genEditObj(curPos, curLetter, "delete", null, null, EditObj);
        });
        //console.log(EditObj);
    })
        .on("mouseout", function (d) {
            //============points and count===================
            //point
            var pointId = "#" + d[1] + "-point";
            var point = $(pointId);
            //count
            var countId = "#" + d[1] + "-count";
            var count = $(countId);

            if (d[1] % 5 === 0) {
                //point is original hidden, do nothing on point
                //test is original show, change the text color back
                count.css({ "fill-opacity": 0.2 });
                count.css({ fill: "#3b3eac" });

            }
            else {

                //point is originaly shown, show it again
                point.css({ display: "inline" });
                //count is originally hiden, now hidden it again
                count.css({ display: "none" });

                //change color back
                count.css({ "fill-opacity": 0.2 });
                count.css({ fill: "#3b3eac" });
            }

        });
}
function toolPieClick(d, ctgt, target_data, color, r, seq_len) {
    //cur data
    var curLetter = target_data[0];
    var curPos = target_data[1];
    var curId = "#" + curPos + "-letter";
    //hide tool
    d3.selectAll(".tool")
      .transition().delay(150)
      .attr("display", "none");
    var ctgtdata = ctgt[0][0].__data__.data;
    var ctype = ctgtdata.type;
    var cletter = ctgtdata.letter;
    //console.log(target_data);
    //decide what to do
    if (ctype == "change") {
        //change to cletter
        //hide tool
        d3.selectAll(".tool")
          .transition().delay(150)
          .attr("display", "none");
        $(curId).addClass("changed")
            .css("text-decoration", "underline")
            .css("text-decoration-style", "double")
            .css("font-size", "15px")
            .css("font-weight", "bold")
            .css("stroke", "#756bb1")
            .css("fill", function () { return color(cletter); })
            .text(cletter);
        //add original letter below
        if ($("#" + target_data[1] + "-oriletter").length == 0) {
            var oletter = d3.select("svg").append("text").attr("class", "oriLetter letter clickable")
                                                .attr("y", target_data[3] + 68)
                                                .attr("x", target_data[2] + 36)
                                                .attr("id", target_data[1] + "-oriletter")
                                                .style("font", "10px Arial")
                                                .style("fill", "#969696")
                                                .text(target_data[0]);
        }
        //update EditObj
        EditObj = genEditObj(curPos, curLetter, "change", null, cletter, EditObj);
        //genEditObj(pos, oriLetter, action, lor, newLetter, EditObj)

    }
    if (ctype == "before") {
        //insert cletter before current
        //hide tool
        d3.selectAll(".tool")
          .transition().delay(150)
          .attr("display", "none");
        //add insert before (left)
        //check whether there is  an insert already
        //add new letter
        var posString = (target_data[1] - 1) + "-" + target_data[1];
        var leftId = posString + "-insert";
        var divId = posString + "-div";

        if ($("#" + leftId).length === 0) {
            var leftgroup = d3.select("svg").append("g")
                .attr("id", leftId)
                .attr("class", "clickable")
                .on("dblclick", function () {
                    //delete this
                    d3.select(this).remove();
                    //hide seq-tooltip
                    $("#" + divId).css("opacity", 0);
                    //delete data object
                    removeEditObj("insert", leftId, EditObj);
                });
            var leftSlash = leftgroup.append("line")
                                     .style("stroke", "gray")
                                     .attr("x1", target_data[2] + 30)
                                     .attr("y1", target_data[3] + 45)
                                     .attr("x2", target_data[2] + 20)
                                     .attr("y2", target_data[3] + 30);
            //apend letter
            var leftLetter = leftgroup.append("text").attr("class", "leftLetter letter clickable")
                                    .attr("y", target_data[3] + 40)
                                    .attr("x", target_data[2] + 27)
                                    .attr("id", posString + "-letter")
                                    .attr("font-size", "15px")
                                    .attr("font-weight", "bold")
                                    .attr("stroke", "#756bb1")
                                    .style("fill", function () { return color(cletter); })
                                    .text(cletter);
            //append line
            var lineId = (target_data[1] - 1) + "-" + target_data[1] + "-line";
            var leftline = leftgroup.append("line").attr("id", lineId).attr("class", "clickable")
                                       .style("stroke", "purple")
                                       .style("stroke-width", 2)
                                       .attr("x1", target_data[2] + 27)
                                       .attr("y1", target_data[3] + 35)
                                       .attr("x2", target_data[2] + 37)
                                       .attr("y2", target_data[3] + 35)
                                       .attr("stroke-dasharray", "2, 2, 2, 2")
                                       .style("opacity", 0);
            //append a div in body
            var divId = (target_data[1] - 1) + "-" + target_data[1] + "-div";
            var insertDiv = d3.select("body").append("div")
                              .attr("id", divId)
                              .attr("class", "seq-tooltip")
                              .style("opacity", 0);
            //dot line event
            leftline.on("mouseover", function () {
                insertDiv.transition()
                 .duration(200)
                 .style("opacity", .9)
                 .style("left", (d3.event.pageX - 40) + "px")
                  .style("top", (d3.event.pageY - 50) + "px");
            })
                    .on("mouseleave", function () {
                        insertDiv.transition()
                            .duration(200)
                            .style("opacity", 0);
                    });
            //update EditObj
            EditObj = genEditObj(curPos, curLetter, "insert", "left", cletter, EditObj);

            //genEditObj(pos, oriLetter, action, lor, newLetter, EditObj)

        }
        else {
            //check whether it is letter for a plus symbol
            if ($("#" + posString + "-letter").length !== 0) {
                //this is a letter
                //get the nletter
                var previous_letter = $("#" + posString + "-letter").text();
                var nletters = previous_letter + cletter;
                //find the div
                var divId = (target_data[1] - 1) + "-" + target_data[1] + "-div";
                //save the letter in the new div
                $("#" + divId).html(function (d) { return "<span class=\"text-info\">" + nletters + "</span>"; });

                //remove leftLetter
                $("#" + posString + "-letter").remove();
                //show dash line
                var lineId = posString + "-line";
                $("#" + lineId).css("opacity", .9);
                //update EditObj
                EditObj = genEditObj(curPos, curLetter, "insert", "left", nletters, EditObj);
            }
            else {
                //this is a plus div
                var divId = posString + "-div";
                var previous_letter = $("#" + divId + " span").text();
                var nletters = previous_letter + cletter;
                $("#" + divId).empty();
                $("#" + divId).html(function (d) { return "<span class=\"text-info\">" + nletters + "</span>"; });
                //update EditObj
                EditObj = genEditObj(curPos, curLetter, "insert", "left", nletters, EditObj);
            }
        }

    }
    if (ctype == "after") {
        //insert cletter after current
        //hide tool
        d3.selectAll(".tool")
          .transition().delay(150)
          .attr("display", "none");


        //add insert before (left)
        //check whether there is  an insert already
        //add new letter
        var posString = target_data[1] + "-" + (target_data[1] + 1);
        var xshift = +2 * r;
        var leftId = posString + "-insert";
        var divId = posString + "-div";
        if ($("#" + leftId).length === 0) {
            var leftgroup = d3.select("svg").append("g")
                .attr("id", leftId)
                .attr("class", "clickable")
                .on("dblclick", function () {
                    //delete this
                    d3.select(this).remove();
                    //hide seq-tooltip
                    $("#" + divId).css("opacity", 0);
                    //delete data object
                    removeEditObj("insert", leftId, EditObj);
                });
            var leftSlash = leftgroup.append("line")
                                     .style("stroke", "gray")
                                     .attr("x1", target_data[2] + 30 + xshift)
                                     .attr("y1", target_data[3] + 45)
                                     .attr("x2", target_data[2] + 20 + xshift)
                                     .attr("y2", target_data[3] + 30);
            //apend letter
            var leftLetter = leftgroup.append("text").attr("class", "leftLetter letter clickable")
                                    .attr("y", target_data[3] + 40)
                                    .attr("x", target_data[2] + 27 + xshift)
                                    .attr("id", posString + "-letter")
                                    .attr("font-size", "15px")
                                    .attr("font-weight", "bold")
                                    .attr("stroke", "#756bb1")
                                    .style("fill", function () { return color(cletter); })
                                    .text(cletter);
            //append line
            var lineId = posString + "-line";
            var leftline = leftgroup.append("line").attr("id", lineId).attr("class", "clickable")
                                       .style("stroke", "purple")
                                       .style("stroke-width", 2)
                                       .attr("x1", target_data[2] + 27 + xshift)
                                       .attr("y1", target_data[3] + 35)
                                       .attr("x2", target_data[2] + 37 + xshift)
                                       .attr("y2", target_data[3] + 35)
                                       .attr("stroke-dasharray", "2, 2, 2, 2")
                                       .style("opacity", 0);
            //append a div in body
            var divId = posString + "-div";
            var insertDiv = d3.select("body").append("div")
                              .attr("id", divId)
                              .attr("class", "seq-tooltip")
                              .style("opacity", 0);
            //dot line event
            leftline.on("mouseover", function () {
                insertDiv.transition()
                 .duration(200)
                 .style("opacity", .9)
                 .style("left", (d3.event.pageX - 40) + "px")
                  .style("top", (d3.event.pageY - 50) + "px");
            })
                    .on("mouseleave", function () {
                        insertDiv.transition()
                            .duration(200)
                            .style("opacity", 0);
                    });
            //update EditObj
            if (curPos == seq_len) {
                //length + 1************************************************************************
                EditObj = genEditObj(curPos + 1, curLetter, "insert", "left", cletter, EditObj);
            }
            else {
                EditObj = genEditObj(curPos, curLetter, "insert", "left", cletter, EditObj);
            }
            //genEditObj(pos, oriLetter, action, lor, newLetter, EditObj)

        }
        else {
            //check whether it is letter for a plus symbol
            if ($("#" + posString + "-letter").length !== 0) {
                //this is a letter
                //get the nletter
                var previous_letter = $("#" + posString + "-letter").text();
                var nletters = cletter + previous_letter;
                //find the div
                var divId = target_data[1] + "-" + (target_data[1] + 1) + "-div";
                //save the letter in the new div
                $("#" + divId).html(function (d) { return "<span class=\"text-info\">" + nletters + "</span>"; });

                //remove leftLetter
                $("#" + posString + "-letter").remove();
                //show dash line
                var lineId = posString + "-line";
                $("#" + lineId).css("opacity", .9);
                //update EditObj
                if (curPos == seq_len) {
                    //length + 1************************************************************************
                    EditObj = genEditObj(curPos + 1, curLetter, "insert", "left", nletters, EditObj);
                }
                else {
                    EditObj = genEditObj(curPos, curLetter, "insert", "left", nletters, EditObj);
                }
            }
            else {
                //this is a plus div
                var divId = posString + "-div";
                var previous_letter = $("#" + divId + " span").text();
                var nletters = cletter + previous_letter;
                $("#" + divId).empty();
                $("#" + divId).html(function (d) { return "<span class=\"text-info\">" + nletters + "</span>"; });
                //update EditObj
                if (curPos == seq_len) {
                    //length + 1************************************************************************
                    EditObj = genEditObj(curPos + 1, curLetter, "insert", "left", nletters, EditObj);
                }
                else {
                    EditObj = genEditObj(curPos, curLetter, "insert", "left", nletters, EditObj);
                }
            }
        }
    }
}
function genArray(num) {
    var arr = [];
    for (var i = 1; i <= num; i++) {
        arr.push(i);
    }
    return arr;
}
function genSeqObj(seqArray, seq_len, numCol, numRow, xRange, yRange) {
    //seqArray

    //position array
    var posArray = genArray(seq_len);
    //cx array and cy array
    var cxArr = [];
    var cyArr = [];

    for (y1 = 0; y1 < numRow; y1++) {
        for (x1 = 0; x1 < numCol; x1++) {
            //left and top padding
            cxArr.push(x1 * xRange + xRange / 2);
            cyArr.push(y1 * yRange + yRange / 2);
        }
    }
    //remove extra items in cxArr and cyArr
    if (cxArr.length > seq_len) {
        cxArr.slice(0, seq_len - 1);
        cyArr.slice(0, seq_len - 1);
    }
    seqObj = d3.zip(seqArray, posArray, cxArr, cyArr);
    return seqObj;
}
function colores_google(n) {
    var colores_g = ["#3366cc", "#dc3912", "#ff9900", "#109618", "#990099", "#0099c6", "#dd4477", "#66aa00", "#b82e2e", "#316395", "#994499", "#22aa99", "#aaaa11", "#6633cc", "#e67300", "#8b0707", "#651067", "#329262", "#5574a6", "#3b3eac"];
    return colores_g[n % colores_g.length];
}
function angle(d) {
    var a = (d.startAngle + d.endAngle) * 90 / Math.PI - 90;
    return a > 90 ? a - 180 : a;
}
//update changeObj
function genEditObj(pos, oriLetter, action, lor, newLetter, EditObj) {
    //pos: -1, // original pos, allow same pos for many times, pos max is seq length + 1
    //oriLetter: "A", //original letter
    //action: "delete", //including, delete, change, insert
    //lor: "left", //for the last neucleotide, we need to add 1;
    //newLetter: "T", //empty for delete, one-letter for change, one or more than one for insert
    //timestamp: "timestamp", edit by ts
    //EditObj is an array for keeping editing info
    var timestamp = $.now();
    //check action
    if (action == "delete") {
        //remove possible previous changes
        rObj = $.grep(EditObj, function (d) {
            return (+d.pos === +pos && d.action == "delete" && d.oriLetter == oriLetter);
        })
        if (rObj.length > 0) {
            $.each(rObj, function (si, sd) {
                var index = $.inArray(sd, EditObj);
                if (index !== -1) {
                    EditObj.splice(index, 1);
                }
            })
        }
        //add new obj
        var obj = {
            "pos": pos,
            "oriLetter": oriLetter,
            "action": action,
            "lor": null,
            "newLetter": null,
            "time": timestamp
        };
        EditObj.push(obj);
    }
    if (action == "change") {
        //remove possible previous changes
        pObj = $.grep(EditObj, function (d) {
            return (+d.pos === +pos && d.action == "change" && d.oriLetter == oriLetter);
        })
        if (pObj.length > 0) {
            $.each(pObj, function (si, sd) {
                var index = $.inArray(sd, EditObj);
                if (index !== -1) {
                    EditObj.splice(index, 1);
                }
            })
        }
        //add new object
        var obj = {
            "pos": pos,
            "oriLetter": oriLetter,
            "action": action,
            "lor": null,
            "newLetter": newLetter,
            "time": timestamp
        };
        EditObj.push(obj);
    }
    if (action == "insert") {
        //remove possible previous changes
        pObj = $.grep(EditObj, function (d) {
            return (+d.pos === +pos && d.action == "insert" && d.oriLetter == oriLetter && d.lor == lor);
        })
        if (pObj.length > 0) {
            $.each(pObj, function (si, sd) {
                var index = $.inArray(sd, EditObj);
                if (index !== -1) {
                    EditObj.splice(index, 1);
                }
            })
        }
        var obj = {
            "pos": pos,
            "oriLetter": oriLetter,
            "action": action,
            "lor": lor,
            "newLetter": newLetter,
            "time": timestamp
        };
        EditObj.push(obj);
    }
    return EditObj;
}
//dblclick to remove an object
function removeEditObj(action, id, EditObj) {
    //get pos
    //id = num - num -insert
    var idArray = id.split("-");
    var pos = +idArray[1];

    var rObj = $.grep(EditObj, function (d) {
        return (+d.pos === +pos && d.action == "insert");
    })
    if (rObj.length > 0) {
        $.each(rObj, function (si, sd) {
            var index = $.inArray(sd, EditObj);
            if (index !== -1) {
                EditObj.splice(index, 1);
            }
        })
    }
    return EditObj;
}
//generate the final seq
function genFinalSeq(oriSeqArray, EditObj) {
    //oriSeqArray, orignal seq arrray
    //EditObj: editing info

    //first generate orignal seq object
    var oriObjArray = [];
    //object array {"oriPos": 1+"", "oriLetter": "A"}
    $.each(oriSeqArray, function (i, d) {
        var obj = { oriPos: (i + 1) + "", oriLetter: d };
        oriObjArray.push(obj);
    })
    $.each(EditObj, function (i, d) {
        //find the position in oriObjArray, since there is deletion, cannot use directly the pos-1 to find index
        var pos = +d.pos;
        //find the index in oriObjArray
        var index = -1;
        $.each(oriObjArray, function (si, sd) {
            if (+sd.oriPos === pos) {
                index = si;
            }
        });
        if (index !== -1) {
            //process this edit info
            if (d.action == "delete") {
                //remove this object
                oriObjArray.splice(index, 1);
            }
            if (d.action == "change") {
                var nletter = d.newLetter;
                //update the letter in oriObjArray
                oriObjArray[index].oriLetter = nletter;
            }
            if (d.action == "insert") {
                var nLetter = d.newLetter;
                if (d.lor == "left") {
                    var nPos = d.pos + "left";
                    var obj = { oriPos: nPos, oriLetter: nLetter };
                    //insert at left to the index
                    oriObjArray.splice(index, 0, obj);
                }
                if (d.lor == "right") {
                    var nPos = d.pos + "right";
                    var obj = { oriPos: nPos, oriLetter: nLetter };
                    //insert at right to the index
                    oriObjArray.splice(index + 1, 0, obj);
                }
            }
        }
    })

    //get the final seq array
    var finalArray = [];
    $.each(oriObjArray, function (i, d) {
        finalArray.push(d.oriLetter);
    })
    return finalArray;
}