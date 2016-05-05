//this js file contains all the function used in the project

//load the data from js SerializeObject
function LoadData(data) {
    document.getElementById("decodeIt").innerHTML = data;
    outPut = document.getElementById("decodeIt").innerText;
    return outPut;
}


//draw maps in div and modal from plasmid idArray
//in an each loop
function drawMaps(idArray, Features, minWidth, modalWidth) {
    $.each(idArray, function (index, value) {
        mapId = "map-" + value;
        modalMapId = "modal-map-" + value;
        //filter featrues
        var currentFeatures = $.grep(Features, function (n, i) {
            return n.pId === +value;
        });
        if (currentFeatures.length > 0) {
            pName = currentFeatures[0].pName;
            pCount = currentFeatures[0].pSeqCount;
            pId = currentFeatures[0].pId;

            delete currentFeatures['pId'];
            delete currentFeatures['pName'];
            delete currentFeatures['pCount'];

            fData = [pCount, currentFeatures];

            width = ($("#" + mapId).width() <= minWidth ? minWidth : $("#" + mapId).width());
            //console.log(width);
            //draw the map
            var gd = GiraffeDraw();
            gd.read(fData);
            gd.CircularMap({
                'map_dom_id': mapId,
                'plasmid_name': pName,
                'map_width': width,
                'map_height': width
            });

            gd.CircularMap({
                'map_dom_id': modalMapId,
                'plasmid_name': pName,
                'map_width': modalWidth,
                'map_height': modalWidth
            });
        }
    });
}

//draw on 2 maps in div and in modal, not in a each loop

function draw2Maps(Features, mapId, modalMapId, d, minWidth, modalWidth) {
    //filter featrues
    var currentFeatures = $.grep(Features, function (n, i) {
        return n.pId === +d;
    });
    if (currentFeatures.length > 0) {
        pName = currentFeatures[0].pName;
        pCount = currentFeatures[0].pSeqCount;
        pId = currentFeatures[0].pId;

        delete currentFeatures['pId'];
        delete currentFeatures['pName'];
        delete currentFeatures['pCount'];

        fData = [pCount, currentFeatures];

        width = ($("#" + mapId).width() <= minWidth ? minWidth : $("#" + mapId).width());
        //draw the map
        var gd = GiraffeDraw();
        gd.read(fData);
        gd.CircularMap({
            'map_dom_id': mapId,
            'plasmid_name': pName,
            'map_width': width,
            'map_height': width
        });

        gd.CircularMap({
            'map_dom_id': modalMapId,
            'plasmid_name': pName,
            'map_width': modalWidth,
            'map_height': modalWidth
        });
    }
};


function draw1Map(Features, mapId, d, width, height) {
    //filter featrues
    var currentFeatures = $.grep(Features, function (n, i) {
        return n.pId === +d;
    });
    if (currentFeatures.length > 0) {
        pName = currentFeatures[0].pName;
        pCount = currentFeatures[0].pSeqCount;
        pId = currentFeatures[0].pId;

        delete currentFeatures['pId'];
        delete currentFeatures['pName'];
        delete currentFeatures['pCount'];

        fData = [pCount, currentFeatures];

        width = ($("#" + mapId).width() <= width ? width : $("#" + mapId).width());
        //draw the map
        var gd = GiraffeDraw();
        gd.read(fData);
        gd.CircularMap({
            'map_dom_id': mapId,
            'plasmid_name': pName,
            'map_width': width,
            'map_height': height
        });
    }
};
//draw maps not only div, not in modal
function drawMap(idArray, Features, width, height) {
    $.each(idArray, function (index, value) {
        mapId = "map-" + value;
        //get the plasmid id
        var pieces = mapId.split('-');
        var plasmidId = +pieces[pieces.length - 1];
        //filter featrues
        //console.log(plasmidId);
        //console.log(Features);
        var currentFeatures = $.grep(Features, function (n, i) {
            return n.pId === plasmidId;
        });
        //console.log(currentFeatures);
        if (currentFeatures.length > 0) {
            pName = currentFeatures[0].pName;
            pCount = currentFeatures[0].pSeqCount;
            pId = currentFeatures[0].pId;

            delete currentFeatures['pId'];
            delete currentFeatures['pName'];
            delete currentFeatures['pCount'];

            fData = [pCount, currentFeatures];

            if ($('#' + mapId).length != 0) {
                //draw the map
                var gd = GiraffeDraw();
                gd.read(fData);
                gd.CircularMap({
                    'map_dom_id': mapId,
                    'plasmid_name': pName,
                    'map_width': width,
                    'map_height': height
                });
            }
        }
    });
}

//sort array
function sortByProperty(property) {
    'use strict';
    return function (a, b) {
        var sortStatus = 0;
        if (a[property] < b[property]) {
            sortStatus = -1;
        } else if (a[property] > b[property]) {
            sortStatus = 1;
        }
        return sortStatus;
    };
}


//click to redraw maps in modal
//need to pass the idarray into the click event
// $('.modal-body div div svg text tspan').click({ idArray: idArray }, redrawMap);
function redrawModalMap(event) {
    var txt = $(event.target).text();
    //find feature clicked
    var idArray = event.data.idArray;
    var Features = event.data.Features;

    $.each(idArray, function (i, d) {
        //mapId = "map-" + d;
        modalMapId = "modal-map-" + d;
        //filer data
        var currentFeatures = $.grep(Features, function (n, i) {
            return n.pId === +d;
        });
        pName = currentFeatures[0].pName;
        pCount = currentFeatures[0].pSeqCount;
        pId = currentFeatures[0].pId;

        delete currentFeatures['pId'];
        delete currentFeatures['pName'];
        delete currentFeatures['pCount'];

        fData = [pCount, currentFeatures];

        var oldStart = 0;
        $.each(fData[1], function (index, d) {
            $.each(d, function (key, value) {
                if (value == txt) {
                    oldStart = d['start'];
                }
            });
        });

        //change the start and end
        $.each(fData[1], function (i, dt) {
            if (oldStart != 0) {
                if (dt['start'] >= oldStart) {
                    dt['start'] = dt['start'] - oldStart + 1;
                    dt['end'] = dt['end'] - oldStart + 1;
                }
                else {
                    dt['start'] = pCount - oldStart + dt['start'] + 1;
                    if (dt['end'] >= oldStart) {
                        dt['end'] = dt['end'] - oldStart + 1;
                    }
                    else {
                        dt['end'] = pCount - oldStart + dt['end'] + 1;
                    }
                }
            }
        });
        //reorder the data
        fData[1].sort(sortByProperty('start'));

        //redraw the map
        var gd = GiraffeDraw();
        gd.read(fData);
        gd.CircularMap({
            'map_dom_id': modalMapId,
            'plasmid_name': pName,
            'map_width': 1048,
            'map_height': 1048
        });
    });
    //run the click-event again
    $('svg text tspan').click({ idArray: idArray, Features: Features }, redrawModalMap);
};


//click to redraw maps not in modal
//need to pass the idarray into the click event
// $('svg text tspan').click({ idArray: idArray, width: width, height:height }, redrawMap);
function redrawMap(event) {
    var txt = $(event.target).text();
    //find feature clicked
    var idArray = event.data.idArray;
    var width = event.data.width;
    var height = event.data.height;
    $.each(idArray, function (i, d) {

        //filer data
        var currentFeatures = $.grep(Features, function (n, i) {
            return n.pId === +d;
        });
        pName = currentFeatures[0].pName;
        pCount = currentFeatures[0].pSeqCount;
        pId = currentFeatures[0].pId;

        delete currentFeatures['pId'];
        delete currentFeatures['pName'];
        delete currentFeatures['pCount'];

        fData = [pCount, currentFeatures];

        var oldStart = 0;
        $.each(fData[1], function (index, d) {
            $.each(d, function (key, value) {
                if (value == txt) {
                    oldStart = d['start'];
                }
            });
        });

        //change the start and end
        $.each(fData[1], function (i, dt) {
            if (oldStart != 0) {
                if (dt['start'] >= oldStart) {
                    dt['start'] = dt['start'] - oldStart + 1;
                    dt['end'] = dt['end'] - oldStart + 1;
                }
                else {
                    dt['start'] = pCount - oldStart + dt['start'] + 1;
                    if (dt['end'] >= oldStart) {
                        dt['end'] = dt['end'] - oldStart + 1;
                    }
                    else {
                        dt['end'] = pCount - oldStart + dt['end'] + 1;
                    }
                }
            }
        });
        //reorder the data
        fData[1].sort(sortByProperty('start'));

        var divId = "map-" + d;
        //redraw the map
        var gd = GiraffeDraw();
        gd.read(fData);
        gd.CircularMap({
            'map_dom_id': divId,
            'plasmid_name': pName,
            'map_width': width,
            'map_height': height
        });
    });
    //run the click-event again
    $('svg text tspan').click({ idArray: idArray, width: width, height: height }, redrawMap);
};


function unique(list) {
    var result = [];
    $.each(list, function (i, e) {
        if ($.inArray(e, result) == -1) result.push(e);
    });
    return result;
};