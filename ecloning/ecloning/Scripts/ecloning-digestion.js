//load the data from js SerializeObject
function LoadData(data) {
    document.getElementById("decodeIt").innerHTML = data;
    outPut = document.getElementById("decodeIt").innerText;
    return outPut;
}
//show the plasmid features
function drawMap(giraffeData, id, name, width) {
    var gd = GiraffeDraw();
    gd.read(giraffeData);
    //draw linear
    gd.LinearMap({
        'map_dom_id': id,
        'plasmid_name': name,
        'map_width': width
        //'map_height': 324  
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