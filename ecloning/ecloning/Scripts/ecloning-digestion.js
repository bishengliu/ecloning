function scrollTo(id) {
    $('html, body').animate({
        scrollTop: $(id).offset().top
    }, 1000);
}
//load the data from js SerializeObject
function LoadData(data) {
    document.getElementById("decodeIt").innerHTML = data;
    outPut = document.getElementById("decodeIt").innerText;
    return outPut;
}
//show the plasmid features
function drawMap(giraffeData, id, name, width, cutArray) {
    var gd = GiraffeDraw();
    gd.read(giraffeData);
    //draw linear
    gd.CircularMap({
        'map_dom_id': id,
        'plasmid_name': name,
        'label_offset': 5,
        'digest': true,
        'map_width': width,
        'map_height': width,
        'cutters': cutArray        
    });
}

//show featrues
function Color(type_id) {
    switch (type_id) {
        case 1:
            return '#1f77b4';
            break;
        case 2:
            return '#ff7f0e';
            break;
        case 3:
            return '#2ca02c';
            break;
        case 4:
            return '#8c564b';
            break;
        case 5:
            return '#d62728';
            break;
        case 6:
            return '#7f7f7f';
            break;
        case 7:
            return '#e377c2';
            break;
        case 8:
            return '#9467bd';
            break;
        case 9:
            return ' #bcbd22';
            break;
        case 10:
            return '#17becf';
            break;
    }
}
function getFeatureData(fvFeatures, array, type_id) {
    $.each($.grep(fvFeatures, function (d) {
        return +d.type_id === type_id;
    }), function (i, n) {
        delete n.type_id;
        array.push(n);
    });

    $.each(array, function (i, d) {
        d.color = Color(type_id);
    });
    return array;
}
function drawFeatures(fvFeatures, seq, id){
    //display features
    //console.log(fvFeatures);
    //preocess the data
    var fArray = [];
    var gFeature = getFeatureData(fvFeatures, fArray, 1); //1

    var fArray = [];
    var promotor = getFeatureData(fvFeatures, fArray, 2); //2

    var fArray = [];
    var primer = getFeatureData(fvFeatures, fArray, 3); //3

    var fArray = [];
    var enzyme = getFeatureData(fvFeatures, fArray, 4); //4

    var fArray = [];
    var gene = getFeatureData(fvFeatures, fArray, 5); //5

    var fArray = [];
    var origin = getFeatureData(fvFeatures, fArray, 6); //6

    var fArray = [];
    var regulatory = getFeatureData(fvFeatures, fArray, 7); //7

    var fArray = [];
    var terminator = getFeatureData(fvFeatures, fArray, 8); //8

    var fArray = [];
    var eFeature = getFeatureData(fvFeatures, fArray, 9); //9

    var fArray = [];
    var orf = getFeatureData(fvFeatures, fArray, 10); //10

    //merge eFeature and gFeature
    mFeature = $.extend(gFeature, eFeature);

    var fOptions = {
        showAxis: true, showSequence: true,
        brushActive: true, toolbar: true,
        bubbleHelp: true, zoomMax: 20
    };
    var ft = new FeatureViewer(seq, "#"+id, fOptions);


    if (enzyme.length > 0) {
        ft.addFeature({
            data: enzyme,
            name: "Restriction Cuts",
            className: "enzyme",
            color: Color(4),
            type: "rect",
            filter: "type2"
        });
    }
    if (regulatory.length > 0) {
        ft.addFeature({
            data: regulatory,
            name: "Regulatory",
            className: "regulatory",
            color: Color(7),
            type: "rect",
            filter: "type2"
        });
    }
    if (promotor.length > 0) {
        ft.addFeature({
            data: promotor,
            name: "Promotor",
            className: "promotor",
            color: Color(2),
            type: "rect",
            filter: "type2"
        });
    }
    if (gene.length > 0) {
        ft.addFeature({
            data: gene,
            name: "Gene",
            className: "gene",
            color: Color(5),
            type: "rect",
            filter: "type2"
        });
    }
    if (terminator.length > 0) {
        ft.addFeature({
            data: terminator,
            name: "Terminator",
            className: "terminator",
            color: Color(8),
            type: "rect",
            filter: "type2"
        });
    }
    if (orf.length > 0) {
        ft.addFeature({
            data: orf,
            name: "ORF",
            className: "orf",
            color: Color(10),
            type: "rect",
            filter: "type2"
        });
    }
    if (mFeature.length > 0) {
        ft.addFeature({
            data: mFeature,
            name: "Generic Feature",
            className: "gFeature",
            color: Color(1),
            type: "rect",
            filter: "type2"
        });
    }
    if (origin.length > 0) {
        ft.addFeature({
            data: origin,
            name: "Origin",
            className: "origin",
            color: Color(6),
            type: "rect",
            filter: "type2"
        });
    }
    if (primer.length > 0) {
        ft.addFeature({
            data: primer,
            name: "Primer",
            className: "primer",
            color: Color(3),
            type: "rect",
            filter: "type2"
        });
    }
}

//display enzyme list
function displayEnzymeList(enzymes, id) {
    if (enzymes.length == 0) {
        $("#enzyme-list").append('<p class="text-danger">No restriction cuts found!</p>');
    }
    else {
        //process the enzymes
        var enzymeData = d3.nest()
                           .key(function (d) { return d.name; })
                           .entries(enzymes);
        //console.log(enzymeData);
        //append checkbox
        $.each(enzymeData, function (i, v) {
            //find methylation 
            var methylation = false;
            $.each(v.values, function (si, sv) {
                if (sv.methylation == true) {
                    methylation = true;
                }
            })
            //add checkbox
            var html = CheckBox(v.key, v.values.length, methylation);
            $("#" + id).append(html);
        })
    }
    
}
function CheckBox(name, count,methylation) {
    var className = methylation?"text-danger":"";
    var html = '<div class="checkbox">';
    html = html + '<label><input type="checkbox" value="' + name + '"><span class="' + className + '">' + name + ' (' + count + ')</span></label>';
    html = html + '</div>';
    return html;
}
function updateArray(value, enzyArray, type)
{
    if (type == "add") {
        enzyArray.push(value);
        unique(enzyArray);
    }
    if (type == "remove") {
        enzyArray = removeElement(enzyArray, value)
    }
    return enzyArray;
}


function updateDigest(value, digestArray, type) {
    if (type == "add") {
        digestArray.push([value]);
    }
    if (type == "remove") {
        digestArray = rmDigestion(digestArray, value);
    }
    return digestArray;
}

function rmDigestion(digestArray, value) {
    array = $.grep(digestArray, function (v) {
        return v.length != 1 || (v.length == 1 && v[0] != value);
    })
    return array;
}


function unique(array) {
    return $.grep(array, function (el, index) {
        return index === $.inArray(el, array);
    });
}
function removeElement(array, value) {
    array = $.grep(array, function (v) {
        return v != value;
    })
    return array;
}

//display enzyme tabs
function genEnzymeTabs(enzyArray, id) {
    if (enzyArray.length > 0) {
        $("#" + id).empty();
        var html = createEnzymeTabs(enzyArray, id);
        $("#" + id).append(html);
        activeTab(enzyArray, 'enzyme-info-tab');
    } else {
        $("#" + id).empty();
    }
}
function createEnzymeTabs(enzyArray, id) {
    //generate ul
    var htmlul = '<ul class="nav nav-tabs" id="enzyme-info-tab">';
    //generate tab-content
    var htmldiv = '<div class="tab-content">';

    $.each(enzyArray, function (i, v) {
        //ul
        var htmlli = '<li><a data-toggle="tab" href="#' + v + '">' + v + '</a></li>';
        htmlul = htmlul + htmlli;
        //div
        var htmlsubdiv = '<div id="' + v + '" class="tab-pane fade">';
        htmlsubdiv = htmlsubdiv + '<div class="panel-group" id="' + 'accordion-'+v+ '">'
                    htmlsubdiv = htmlsubdiv + '<div class="panel panel-primary">';
                        htmlsubdiv = htmlsubdiv + '<div class="panel-heading">';
                            htmlsubdiv = htmlsubdiv + '<h4 class="panel-title">';
                            htmlsubdiv = htmlsubdiv + '<a data-toggle="collapse" data-parent="#' + 'accordion-' + v + '" href="#' + v + '1">'+v+' Property</a>';
                            htmlsubdiv = htmlsubdiv + '</h4>';
                        htmlsubdiv = htmlsubdiv + '</div>';
                        htmlsubdiv = htmlsubdiv + '<div id="'+v+'1" class="panel-collapse collapse">';
                            htmlsubdiv = htmlsubdiv + '<div class="panel-body" id="'+v+'-cut-property"></div>';
                        htmlsubdiv = htmlsubdiv + '</div>';                        
                    htmlsubdiv = htmlsubdiv + '</div>';

                    
                    htmlsubdiv = htmlsubdiv + '<div class="panel panel-primary">';
                        htmlsubdiv = htmlsubdiv + '<div class="panel-heading">';
                            htmlsubdiv = htmlsubdiv + '<h4 class="panel-title">';
                                htmlsubdiv = htmlsubdiv + '<a data-toggle="collapse" data-parent="#' + 'accordion-' + v + '" href="#' + v + '2">' + v + ' Cuts</a>';
                            htmlsubdiv = htmlsubdiv + '</h4>';
                        htmlsubdiv = htmlsubdiv + '</div>';
                        htmlsubdiv = htmlsubdiv + '<div id="' + v + '2" class="panel-collapse collapse in">';
                            htmlsubdiv = htmlsubdiv + '<div class="panel-body" >';
                                htmlsubdiv = htmlsubdiv + '<div id="' + v + '-cut-map' + '"></div>';
                                htmlsubdiv = htmlsubdiv + '<div id="' + v + '-cut-info' + '"></div>'; //add cut position and methylation info
                            htmlsubdiv = htmlsubdiv + '</div>';
                        htmlsubdiv = htmlsubdiv + '</div>';
                   htmlsubdiv = htmlsubdiv + '</div>';

               htmlsubdiv = htmlsubdiv + '</div>';
           htmlsubdiv = htmlsubdiv + '</div>';

        htmldiv = htmldiv + htmlsubdiv;
    });

    htmlul = htmlul + '</ul>';
    htmldiv = htmldiv + '</div>';
    return htmlul + htmldiv;
}


function activeTab(enzyArray, id){
    //find the fist tab
    var tab = enzyArray[enzyArray.length-1];
    $('#'+id+' a[href="#' + tab + '"]').tab('show');
}


//draw cut map
function drawCutMap(enzymes, enzyArray, features, seqCount, name) {
    //genCut array
    var cutArray = [];
    //find the max of the cuts
    var maxCuts = findMaxCuts(enzymes);
    cutArray = genArray(maxCuts);
    //draw cut map
    $.each(enzyArray, function (i, v) {
        var width = 350;
        //fiter features
        var ftdata = $.grep(features, function (d) {
            return d.type_id != 4 || (d.type_id == 4 && d.feature == v);
        });
        var giraffeData = [seqCount, ftdata];
        id = v + "-cut-map";
        drawMap(giraffeData, id, name + ': ' + v, width, cutArray);
    });
}
function genArray(maxCuts) {
    var array = [];
    for (var i = 1; i <= maxCuts; i++) {
        array.push(i);
    }
    return array;
}
function findMaxCuts(enzymes) {
    //process the enzymes
    var enzymeData = d3.nest()
                       .key(function (d) { return d.name; })
                       .entries(enzymes);
    var maxCuts = d3.max(enzymeData, function (d) {
        return d.values.length;
    });
    return maxCuts;
}
//display cut property
//show restriction property
function showRestricProperty(enzyArray, restricProperty) {
    $.each(enzyArray, function (i, v) {
        var id = v + "-cut-property";
        var data = $.grep(restricProperty, function (value, index) {
            return value.name === v;
        })
        $("#" + id).append("<h4 class='text-danger'>Prototype</h4><div>" + data[0].prototype + "</div>");
        //append list group
        var listGroup = "<br/><h4 class='text-danger'>Properties</h4>";
        listGroup = listGroup + '<div class="table-responsive">';
            listGroup = listGroup + '<table class="table table-condensed">';
                listGroup = listGroup + '<tr>';
                    listGroup = listGroup + '<th><span class="fa fa-star text-primary"></span></th>';
                    listGroup = listGroup + '<th><span class="glyphicon glyphicon-fire text-primary"></span></th>';
                    listGroup = listGroup + '<th><span class="text-primary">Dam</span></th>';
                    listGroup = listGroup + '<th><span class="text-primary">Dcm</span></th>';
                    listGroup = listGroup + '<th><span class="text-primary">CpG</span></th>';
                listGroup = listGroup + '</tr>';
                listGroup = listGroup + '<tr>';
                    listGroup = listGroup + '<td>' + data[0].startActivity + '</td>';
                    listGroup = listGroup + '<td>' + data[0].heatInactivation + '</td>';
                    listGroup = listGroup + '<td>' + data[0].dam + '</td>';
                    listGroup = listGroup + '<td>' + data[0].dcm + '</td>';
                    listGroup = listGroup + '<td>' + data[0].cpg + '</td>';
                listGroup = listGroup + '</tr>';
            listGroup = listGroup + '</table>';
        listGroup = listGroup + '</div>';
        $("#" + id).append(listGroup);
    })
}

//add cut positions
function showDigestInfo(enzymes, enzyArray, methylation) {
    $.each(enzyArray, function (i, v) {
        var cuts = [];
        cuts = $.grep(enzymes, function (d, si) {
            return d.name === v;
        });
        
        if (cuts.length > 0) {
            var html = "<br/><h4 class='text-primary'>Digestion:</h4>";
            var pos = [];
            //sort cuts
            cuts.sort(sortByProperty('cut'));
            $.each(cuts, function (ci, cd) {
                if (cd.methylation) {
                    var posString = "" + cd.cut;
                    //get the methlation info
                    var methy = $.grep(methylation, function (md, mi) {
                        return md.cut === cd.cut && md.name === cd.name;
                    })
                    if (methy.length > 0) {
                        if (methy[0].dam_cm || methy[0].dam_ip) {
                            posString = '<span class="text-danger">' + posString + " (Dam)" + "</span>";
                        }
                        if (methy[0].dcm_cm || methy[0].dcm_ip) {
                            posString = '<span class="text-danger">' + posString + " (Dcm)" + "</span>";
                        }
                    }
                    pos.push(posString);
                }
                else {
                    pos.push("" + cd.cut);
                }
            });
            //convert pos array to string
            html = html + "<br/><p>";
            html = html + pos.join(", ");
            html = html + "</p>";
            //append html
            $("#" + v + '-cut-info').append(html);
        }            
    })
}



//gen avtivity info tab
function genActivityTab(enzyArray, id, company){
    if (enzyArray.length > 0) {
        $("#" + id).empty();
        var html = createActivityTabs(enzyArray, id, company);
        $("#" + id).append(html);
        activeActivityTab(enzyArray, 'activity-info-tab');
    } else {
        $("#" + id).empty();
    }
}

function activeActivityTab(enzyArray, id) {
    //find the fist tab
    var tab = enzyArray[enzyArray.length - 1];
    $('#' + id + ' a[href="#activity-' + tab + '"]').tab('show');
}

function createActivityTabs(enzyArray, id, company) {
    //generate ul
    var htmlul = '<ul class="nav nav-tabs" id="activity-info-tab">';
    //generate tab-content
    var htmldiv = '<div class="tab-content">';

    $.each(enzyArray, function (i, v) {
        //ul
        var htmlli = '<li><a data-toggle="tab" href="#activity-' + v + '">' + v + '</a></li>';
        htmlul = htmlul + htmlli;
        //div
        var htmlsubdiv = '<div id="activity-' + v + '" class="tab-pane fade">';
        htmlsubdiv = htmlsubdiv + '<div class="panel-group" id="' + 'accordion-activity-' + v + '">'
            
        $.each(company, function (si, sv) {
                    htmlsubdiv = htmlsubdiv + '<div class="panel panel-primary">';
                        htmlsubdiv = htmlsubdiv + '<div class="panel-heading">';
                            htmlsubdiv = htmlsubdiv + '<h4 class="panel-title">';
                            htmlsubdiv = htmlsubdiv + '<a data-toggle="collapse" data-parent="#' + 'accordion-activity-' + v + '" href="#activity-' + v + '-'+ si + '">' + sv + '</a>';
                            htmlsubdiv = htmlsubdiv + '</h4>';
                        htmlsubdiv = htmlsubdiv + '</div>';
                        htmlsubdiv = htmlsubdiv + '<div id="activity-' + v + '-' + si + '" class="panel-collapse collapse">';
                        htmlsubdiv = htmlsubdiv + '<div class="panel-body" id="company-' + v + '-' + si + '"></div>';
                        htmlsubdiv = htmlsubdiv + '</div>';                        
                    htmlsubdiv = htmlsubdiv + '</div>';
        })

               htmlsubdiv = htmlsubdiv + '</div>';
           htmlsubdiv = htmlsubdiv + '</div>';
        htmldiv = htmldiv + htmlsubdiv;
    });

    htmlul = htmlul + '</ul>';
    htmldiv = htmldiv + '</div>';
    return htmlul + htmldiv;
}

function FormatActivity(activity) {
    //format activity
    $.each(activity, function (i, v) {
        v.activity = convertActivity(+v.activity);
    })
    var nestData = d3.nest()
                     .key(function (d) { return d.company; })
                     .key(function (d) { return d.name; })
                     .entries(activity);
    return nestData;
}

function findCompany(activity) {
    var company = [];
    $.each(activity, function (i, v) {
        company.push(v.key);
    })
    return company;
}

function convertActivity(num) {
    var text = "<span></span>";
    if (num == 0) {
        text = "<span><10%</span>";
    } else if (num == 1) {
        text = "<span>10%</span>";
    }
    else if (num == 2) {
        text = "<span>10-25%</span>";
    }
    else if (num == 3) {
        text = "<span>25%</span>";
    }
    else if (num == 4) {
        text = "<span>25-50%</span>";
    }
    else if (num == 5) {
        text = "<span>50%</span>";
    }
    else if (num == 6) {
        text = "<span>50-75%</span>";
    }
    else if (num == 7) {
        text = "<span>75%</span>";
    }
    else if (num == 9) {
        text = "<span>100%</span>";
    }
    return text;
}

function showActivity(enzyArray, activity, company) {
    //loop enzymeArray
    $.each(enzyArray, function (ei, ev) {
        var idArray = [];
        //loop company
        $.each(company, function (ci, cv) {
            var id = 'company-' + ev + '-' + ci;
            idArray.push(id);
            var data1 = $.grep(activity, function (d, i) {
                return d.key === cv;
            });
            if (data1.length > 0) {
                var data2 = $.grep(data1[0].values, function (d, i) {
                    return d.key == ev;
                });
            }
            else{
                data2 = [];
            }
            //generate activity
            dispatchActivity(data2, id);
        })
    })   
}

function dispatchActivity(data, id) {
    var html = "";
    if (data.length > 0) {
        html = html + '<h4 class="text-danger">Enzyme Activity (' + data[0].values[0].temprature + '&deg;C)</h4>';
        html = html + '<div class="table-responsive">';
            html = html + '<table class="table table-condensed">';
                html = html + '<tr>';
                    $.each(data[0].values, function (i, v) {
                        html = html + '<th><span class="text-primary">'+v.buffer+'</span></th>';
                    })
                html = html + '</tr>';
                html = html + '<tr>';
                    $.each(data[0].values, function (i, v) {
                        html = html + '<td><span class="">' + v.activity + '</span></td>';
                    })
                html = html + '</tr>';
            html = html + '</table>';
        html = html + '</div>';
    }
    else {
        html = "<p class='text-danger'>Not found!</p>";
    }

    $("#" + id).append(html);
}


//gen gel
//ladder
function formatLadder(ladders) {
    //d3 nest by id
    var data = d3.nest()
                 .key(function (d) { return d.id; })
                 .entries(ladders);
    $.each(data, function (i, d) {
        var Rf = [];
        var size = [];
        var mass = [];
        var logSize = [];
        $.each(d.values, function (si, sd) {
            Rf.push(sd.Rf); //Rf ~ log(size)
            size.push(sd.size); 
            mass.push(sd.mass);
            logSize.push(+Math.log10(sd.size).toFixed(3));
        })
        d.name = d.values[0].name;
        d.type = "ladder";
        d.Rf = Rf;
        d.Size = size;
        d.Mass = mass;
        d.logSize = logSize;
        d.lr = linearRegression(logSize, Rf); //return [m, b]
    })
    return data;
}
//linear regression by finding least squares
function linearRegression(y, x) {
    var lr = {};
    var n = y.length;
    var sum_x = 0;
    var sum_y = 0;
    var sum_xy = 0;
    var sum_xx = 0;
    var sum_yy = 0;

    for (var i = 0; i < y.length; i++) {

        sum_x += x[i];
        sum_y += y[i];
        sum_xy += (x[i] * y[i]);
        sum_xx += (x[i] * x[i]);
        sum_yy += (y[i] * y[i]);
    }

    lr['slope'] = (n * sum_xy - sum_x * sum_y) / (n * sum_xx - sum_x * sum_x); //m
    lr['intercept'] = (sum_y - lr.slope * sum_x) / n; //b
    lr['r2'] = Math.pow((n * sum_xy - sum_x * sum_y) / Math.sqrt((n * sum_xx - sum_x * sum_x) * (n * sum_yy - sum_y * sum_y)), 2);

    return lr;
}

//generate bands for only one digest enzyme
function genBands(enzyArray, enzymes, methylation, seqCount) {
    var bands = [];
    $.each(enzyArray, function (i, d) {
        var cuts = $.grep(enzymes, function (sd, si) {
            return sd.name === d;
        })
        cuts.sort(sortByProperty('cut'));
        //don't conside the dcm or dam
        var hasMethylation = false;
        if (cuts.length > 0) {
            var curBands = [];
            for (var i = 0; i < cuts.length; i++) {
                if (cuts[i].methylation == true) {
                    //tag this to add an extra lane on gel
                    hasMethylation = true;
                }
                var obj = {};
                obj.name = d;
                obj.type = "cut";
                if (i == cuts.length - 1) {
                    obj.bandRange = cuts[cuts.length - 1].cut == seqCount ? '1-' + cuts[0].cut : cuts[cuts.length - 1].cut + '-' + cuts[0].cut;
                    obj.Size = cuts[cuts.length - 1].cut == seqCount ? cuts[0].cut : (seqCount - cuts[cuts.length - 1].cut + cuts[0].cut);
                    obj.Mass = 100;
                }
                else {
                    obj.bandRange = cuts[i].cut + '-' + cuts[i + 1].cut;
                    obj.Size = cuts[i + 1].cut - cuts[i].cut;
                    obj.Mass = 100;
                }
                obj.logSize = +Math.log10(obj.Size).toFixed(3);
                curBands.push(obj);
            } //end of for loop
            if (curBands.length > 0) {
                bands.push(curBands);
            }

            //deal with dam or dcm
            //add extra lane if methylation is true
            if (hasMethylation) {
                //grep the cur enzyme related methylation
                var methy = $.grep(methylation, function (d, i) {
                    return d.name === cuts[0].name;
                })
                if (methy.length > 0) {
                    var methyBands = []; //final return array

                    var cutsCopy = []; //temp array, for removing completely blocked
                    //remove the completely block digestion
                    for (var i = 0; i < methy.length; i++) {
                        //assume completely blocked
                        if (methylation[i].dam_cm || methylation[i].dcm_cm || methylation[i].dam_ip || methylation[i].dcm_ip) {
                            //remove the cut that have the same cut position
                            cutsCopy = $.grep(cuts, function (d, i) {
                                return +d.cut !== +methylation[i].cut;
                            })
                        }
                    }
                    if (cutsCopy.length === 0) {
                        //nothing left, generate the circular plasmid
                        var obj = {};
                        obj.name = cuts[0].name;
                        obj.type = "cut";
                        obj.bandRange = "0-0"; //if "0-0", then it is circular
                        obj.Size = seqCount;
                        obj.Mass = 100;
                        obj.logSize = +Math.log10(obj.Size).toFixed(3);
                        methyBands.push(obj);
                    }
                    else {
                        //assume always completely cut
                        for (var i = 0; i < cutsCopy.length; i++){
                            var obj = {};
                            obj.name = cutsCopy[i].name;
                            obj.type = "cut";
                            if (i == cuts.length - 1) {
                                obj.bandRange = cuts[cuts.length - 1].cut == seqCount ? '1-' + cuts[0].cut : cuts[cuts.length - 1].cut + '-' + cuts[0].cut;
                                obj.Size = cuts[cuts.length - 1].cut == seqCount ? cuts[0].cut : (seqCount - cuts[cuts.length - 1].cut + cuts[0].cut);
                                obj.Mass = 100;
                            }
                            else {
                                obj.bandRange = cuts[i].cut + '-' + cuts[i + 1].cut;
                                obj.Size = cuts[i + 1].cut - cuts[i].cut;
                                obj.Mass = 100;
                            }
                            obj.logSize = +Math.log10(obj.Size).toFixed(3);
                            methyBands.push(obj);
                        }
                    }

                    if (methyBands.length > 0) {
                        bands.push(methyBands);
                    }
                } //end of methy.length > 0

            } //end of hasMethylation

        } //end of cuts.length


    })

    return bands;
}

//generate digest bands from an enyzme array of arrays, allow multiple enzyme digestion
function genDigestBands(digestArray, enzymes, seqCount) {
    var bands = [];
    //loop the digestArray
    $.each(digestArray, function (i, d) {
        
        //enzymes are all the cuts for the current plasmid
        var cuts = $.grep(enzymes, function (sd, si) {
            //return all the cuts in d, array of digesiton enzymes
            return $.inArray(sd.name, d) != -1;
        })
        //sort cuts by the cut position from small to big
        cuts.sort(sortByProperty('cut'));
        //console.log(cuts);

        
        var hasMethylation = false; //tag for methylation later on
        if (cuts.length > 0) {
            //-------------------------first ignore the methylation
            var curBands = [];
            for (var i = 0; i < cuts.length; i++) {
                if (cuts[i].methylation == true) {
                    //tag this to add an extra lane on gel
                    hasMethylation = true;
                }
                var obj = {};
                obj.name = d.join("-");
                obj.type = "cut";
                if (i == cuts.length - 1) {
                    obj.bandRange = cuts[cuts.length - 1].cut == seqCount ? '1-' + cuts[0].cut : cuts[cuts.length - 1].cut + '-' + cuts[0].cut;
                    obj.Size = cuts[cuts.length - 1].cut == seqCount ? cuts[0].cut : (seqCount - cuts[cuts.length - 1].cut + cuts[0].cut);
                    obj.Mass = 100;
                }
                else {
                    obj.bandRange = cuts[i].cut + '-' + cuts[i + 1].cut;
                    obj.Size = cuts[i + 1].cut - cuts[i].cut;
                    obj.Mass = 100;
                }
                obj.logSize = +Math.log10(obj.Size).toFixed(3);
                curBands.push(obj);
            }//end of for loop
            if (curBands.length > 0) {
                bands.push(curBands);
            }

            //---------------------------- deal with methylation
            if (hasMethylation) {
                var methyBands = []; //final return array
                //remove all the cuts in cuts that has methylation ==true
                var cutsCopy = []; //temp array, for removing completely blocked
                cutsCopy = $.grep(cuts, function (v) {
                    return v.methylation != true;
                });

                if (cutsCopy.length === 0) {
                    //nothing left, generate the circular plasmid
                    var obj = {};
                    obj.name = d.join("-");
                    obj.type = "cut";
                    obj.bandRange = "0-0"; //if "0-0", then it is circular
                    obj.Size = seqCount;
                    obj.Mass = 100;
                    obj.logSize = +Math.log10(obj.Size).toFixed(3);
                    methyBands.push(obj);
                }
                else {
                    //assume always completely cut
                    for (var i = 0; i < cutsCopy.length; i++) {
                        var obj = {};
                        obj.name = d.join("-");
                        obj.type = "cut";
                        if (i == cuts.length - 1) {
                            obj.bandRange = cuts[cuts.length - 1].cut == seqCount ? '1-' + cuts[0].cut : cuts[cuts.length - 1].cut + '-' + cuts[0].cut;
                            obj.Size = cuts[cuts.length - 1].cut == seqCount ? cuts[0].cut : (seqCount - cuts[cuts.length - 1].cut + cuts[0].cut);
                            obj.Mass = 100;
                        }
                        else {
                            obj.bandRange = cuts[i].cut + '-' + cuts[i + 1].cut;
                            obj.Size = cuts[i + 1].cut - cuts[i].cut;
                            obj.Mass = 100;
                        }
                        obj.logSize = +Math.log10(obj.Size).toFixed(3);
                        methyBands.push(obj);
                    }
                }//end of cutsCopy.length
                if (methyBands.length > 0) {
                    bands.push(methyBands);
                }

            }//end of hasMethylation
        } //end of cuts.length
    }) // end of looping digestarray

    return bands;
}


//sort array
//array.sort(sortByProperty(property))
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

//convert each enzyme in the list into an array and prepare for mutiple enzyme digestion
function convertArray(enzyArray) {
    var digestArray = [];
    $.each(enzyArray, function (i, d) {
        digestArray.push([d]);
    })
    return digestArray;
}