    function compareMaps() {
    //alert("submitted!");
    $('#compare-type').text("maps");
    event.preventDefault();
    var idString = $('#plasmid').val();
    if (!idString.trim()) {
        $('#error').text("You haven't selected any plasmid!");
        $('#msg').text(null);
    }
    //get id list and then generate maps and remove duplicates
    var idArray = unique(idString.split(','));
    //remove all the divs
    $("#maps").empty();
    $("#features").empty();

    //append dom to show map
    $.each(idArray, function (i, d) {
        //add dom
        var mapDiv = $("#maps").append("<div id=\"map-" + d + "\" class=\"col-xs-12 col-sm-6 col-md-4 col-lg-4\"></div>");

        var mapId = "map-" + d;
        draw1Map(Features, mapId, d, 568, 648);
    });

    //redraw maps
    $('svg text tspan').click({ idArray: idArray, width: 568, height: 648 }, redrawMap);
    $('#error').text(null);
    $('#msg').text('Click the feature name to rotate the maps.');

};

    function compare(event) {
        //alert("submitted!");
        $('#compare-type').text("maps");
        event.preventDefault();
        var idString = $('#plasmid').val();
        if (!idString.trim()) {
            $('#error').text("You haven't selected any plasmid!");
            $('#msg').text(null);
        }
        //get id list and then generate maps and remove duplicates
        var idArray = unique(idString.split(','));
        //remove all the divs
        $("#maps").empty();
        $("#features").empty();
        
        //append dom to show map
        $.each(idArray, function (i, d) {
            //add dom
            var mapDiv = $("#maps").append("<div id=\"map-" + d + "\" class=\"col-xs-12 col-sm-6 col-md-4 col-lg-4\"></div>");

            var mapId = "map-" + d;
            draw1Map(Features, mapId, d, 568, 648);
        });

        //redraw maps
        $('svg text tspan').click({ idArray: idArray, width: 568, height: 648 }, redrawMap);
        $('#error').text(null);
        $('#msg').text('Click the feature name to rotate the maps.');

    };

    function compareFeature(event) {
        $('#compare-type').text("features");
        event.preventDefault();
        var idString = $('#plasmid').val();
        if (!idString.trim()) {
            $('#error').text("You haven't selected any plasmid!");
            $('#msg').text(null);
        }

        //remove all the divs
        $("#maps").empty();
        $("#features").empty();
        //get id list and then generate maps and remove duplicates
        var idArray = unique(idString.split(','));       
        drawFeatures(idArray, Features, fLabels);
    }

    function drawFeatures(idArray, Features, fLabels) {
        //var margin = { top: 20, right: 200, bottom: 0, left: 20 },
        //    width = ($("#features").width() <= 250 ? 250 : $("#features").width()) - 20 - margin.left - margin.right;
        //height = 650;
        //var padding = 0.5;
        var margin = { top: 20, right: 200, bottom: 0, left: 20 };
        var step = 22,
            width = step * fLabels.length < $("#features").width() - 20 - margin.left - margin.right?$("#features").width() - 20 - margin.left - margin.right : step * fLabels.length;
        height = 650;
        var padding = 0.5;
        //filter the plasmid
        var data = $.grep(Features, function (v, i) {
            return $.inArray("" + v.pId, idArray) !== -1 && v.feature != "undefined";
        });
        var labels = [];
        $.each(data, function (i, d) {
            var item = $.grep(fLabels, function (v, si) {
                return v == d.feature;
            });
            labels.push(item[0]);
        });
        var x = d3.scale.ordinal()
            .domain(labels)
            .rangeRoundPoints([0, width], padding);

        var xAxis = d3.svg.axis()
                    .scale(x)
                    .orient("top");

        var nData = d3.nest()
                  .key(function (d) { return d.pName; })
                  .key(function (d) { return d.pSeqCount; })
                   .entries(data);
        height = 80 + nData.length * 25;
        var svg = d3.select("#features").append("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
            .style("margin-left", margin.left + "px")
            .append("g")
            .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

        var c = nData.length <= 10 ? d3.scale.category20() : d3.scale.category20c();

        //feature tooltip
        var tooltip = d3.select("body")
            .append("div")
            .attr('class', 'feature-tip')
            .style("position", "absolute")
            .style("z-index", "10")
            .style("opacity", 0);

        svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + 80 + ")")
            .call(xAxis)
            .selectAll("text")
            .attr("class", "tickLable")
            .attr("y", -10)
            .attr("x", 10)
            .attr("dy", ".35em")
            .attr("transform", "rotate(-45)")
            .style("text-anchor", "start")
            .on("click", function (event) { sortPlasmid(event, nData, labels, svg, c, x, width); })
            .on("mouseover", mouseoverTickText)
            .on("mouseout", mouseoutTickText);


        draw(nData, labels, svg, x, c, width);

    }

    function draw(nData, labels, svg, x, c, width) {

        for (var j = 0; j < nData.length; j++) {
            var g = svg.append("g").attr("class", "plasmid")
                    .data(nData[j]['values'][0]['values'])
                    .attr("id", function (d) { return "plasmid" + d['pId']; })
                    .attr("transform", "translate(0," + 80 + ")");


            var circles = g.selectAll("circle")
                .data(nData[j]['values'][0]['values'])
                .enter()
                .append("circle");

            var text = g.selectAll("text")
                .data(nData[j]['values'][0]['values'])
                .enter()
                .append("text");

            var rScale = d3.scale.linear()
                .domain([0, d3.max(nData[j]['values'][0]['values'], function (d) { return d['fLength']; })])
                .range([2, 9]);

            var rpScale = d3.scale.linear()
                .domain([0, 1])
                .range([0, 100]);

            //circles
            circles
                .attr("cx", function (d, i) { return x(d['feature']); })
                .attr("cy", j * 20 + 20)
                .attr("r", function (d) { return rScale(d['fLength']); })
                .attr("class", function (d, i) { return d['feature']; })
                .style("fill", function (d) { return c(j); })
                .on("mouseover", function (d) {
                    tooltip.transition()
                         .duration(200)
                         .style("opacity", .9);
                    tooltip.html(d['feature'])
                             .style("left", (d3.event.pageX + 10) + "px")
                             .style("top", (d3.event.pageY - 50) + "px");
                })
                .on("mouseout", function (d) {
                    tooltip.transition()
                         .duration(500)
                         .style("opacity", 0);
                });

            //plasmid length % numbers
            text
                .attr("y", j * 20 + 25)
                .attr("x", function (d, i) { return x(d['feature']); })
                .attr("class", "value")
                .text(function (d) { return d3.round(rpScale(d['fLength'] / nData[j]['values'][0]['key']), 1); })
                .style("fill", function (d) { return c(j); })
                .style("display", "none");

            //plasmid title
            g.append("text")
                .attr("y", j * 20 + 25)
                .attr("x", width + 20)
                .attr("class", "label")
                .attr("id", function (d) { return nData[j]['key'] + "-" + d['pId']; }) //there is a dash between
                .text(truncate(nData[j]['key'] + " (" + nData[j]['values'][0]['key'] + "bp)"), 20, "...")
                .style("fill", function (d) { return c(j); })
                .on("click", function (event) { sortTick(event, nData, labels, svg, c, x, width);})
                .on("mouseover", mouseover)
                .on("mouseout", mouseout);

        };
    }

    function mouseover(p) {

        var g = d3.select(this).node().parentNode;
        d3.select(g).selectAll("circle").style("display", "none");
        d3.select(g).selectAll("text.value").style("display", "block");
    }

    function mouseout(p) {

        var g = d3.select(this).node().parentNode;
        d3.select(g).selectAll("circle").style("display", "block");
        d3.select(g).selectAll("text.value").style("display", "none");
    }

    function mouseoverTickText(p) {
        d3.select(this)
            .style("font-size", "12px")
            .style("fill", "red");
    }
    function mouseoutTickText(p) {
        d3.select(this)
            .style("font-size", "10px")
            .style("fill", "#999");
    }

    function sortPlasmid(event, nData, labels, svg, c, x, width) {
        var tg = d3.event.target;
        var txt = d3.select(tg).text();
        var fNumber = d3.format(".10f");
        //sort nData
        nData.forEach(function (d, i) {
            d['values'][0]['values'].forEach(function (fd, fi) {
                fd.order = 0;
                if (fd['feature'] === txt) {
                    fd.order = fNumber(fd.fLength / fd.pSeqCount);
                }
            });
            //move the clicked feature to the first feature in the plasmid, if it has the feature
            d['values'][0]['values'].sort(function (a, b) { return b['order'] - a['order']; })
        });

        //sort plasmid
        nData.sort(function (a, b) { return b['values'][0]['values'][0]['order'] - a['values'][0]['values'][0]['order']; });

        //remove previous chart
        d3.selectAll(".plasmid").remove();
        //redra the chart
        draw(nData, labels, svg, x, c, width);

    }

    function sortTick(event, nData, labels, svg, c, x, width) {
        var tg = d3.event.target;
        var txt = d3.select(tg).property("id");
        //get the plasmid Id 
        var plasmidId = txt.split('-');
        var pId = parseInt(plasmidId[plasmidId.length - 1]);
        //get all the feaures of this plasmid
        var pFeatures = [];
        nData.forEach(function (d, i) {
            if (d['values'][0]['values'][0]['pId'] === pId) {
                d['values'][0]['values'].forEach(function (fd, fi) {
                    pFeatures.push(fd['feature']);
                });
            };
        });
        //remove the pFeature from the fLabels
        var newLabels = [];
        labels.forEach(function (d, i) {
            if ($.inArray(d, pFeatures) == -1) {
                newLabels.push(d);
            }
        });
        //add the pFeature to the newLabels
        pFeatures.forEach(function (d, i) {
            newLabels.push(d);
        });
        labels = newLabels;

        //update the x.domain() and axis
        var xAxis = d3.svg.axis()
                        .scale(x)
                        .orient("top");
        x.domain(labels);
        xAxis.scale(x);

        svg.call(xAxis).selectAll("text")
        .attr("class", "tickLable")
        .attr("y", -10)
        .attr("x", 10)
        .attr("dy", ".35em")
        .attr("transform", "rotate(-45)")
        .style("text-anchor", "start")
        .on("click", function (event) { sortPlasmid(event, nData, labels, svg, c, x, width); })
        .on("mouseover", mouseoverTickText)
        .on("mouseout", mouseoutTickText);

        //remove previous chart
        d3.selectAll(".plasmid").remove();
        //redra the chart
        draw(nData, labels, svg, x, c, width);
    }