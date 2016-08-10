
function LoadData(data) {
    document.getElementById("decodeIt").innerHTML = data;
    outPut = document.getElementById("decodeIt").innerText;
    return outPut;
}

//draw linear feature map of fragment
function drawLinearMap(features, id, name, size, width) {
    var gd = GiraffeDraw();
    var data = [size, features];
    gd.read(data);
    //draw linear
    gd.LinearMap({
        'map_dom_id': id,
        'plasmid_name': name,
        'digest': false,
        'map_width': width
        //'map_height': width
    });
}

//draw fragment end
function drawEndSeq(id, fSeq, cSeq, overhangs, numLetter) {
    //numLetter is the max letter display on both end

    //left end seqs
    var leftfSeq, leftcSeq, leftSpace;
    if (overhangs[0] > 0) {
        leftfSeq = fSeq.substring(0, numLetter - overhangs[0]);
        leftcSeq = cSeq.substring(0, numLetter);
        leftSpace = genSpace(overhangs[0]);
    }
    else {
        leftfSeq = fSeq.substring(0, numLetter);
        leftcSeq = cSeq.substring(0, numLetter + overhangs[0]);
        leftSpace = genSpace(-overhangs[0]);
    }

    //right end seqs
    var fSeqCount = fSeq.length;
    var cSeqCount = cSeq.length;
    var rightfSeq, rightcSeq, rightSpace;
    if (overhangs[1] > 0) {
        rightfSeq = fSeq.substring(fSeqCount - (numLetter - overhangs[1]), fSeqCount);
        rightcSeq = cSeq.substring(cSeqCount - numLetter, cSeqCount);
        rightSpace = genSpace(overhangs[1]);
    }
    else {
        rightfSeq = fSeq.substring(fSeqCount - numLetter, fSeqCount);
        rightcSeq = cSeq.substring(cSeqCount - (numLetter + overhangs[1]), cSeqCount);
        rightSpace = genSpace(-overhangs[1]);
    }
    //display end in pre
    var html = "";
    var middleDots = "  ......  ";
    var forward = "";
    var complementary = "";
    if (overhangs[0] > 0) {
        forward = "<pre><span class='seqFont'>5' - " + leftSpace + leftfSeq + "</span>" + middleDots;
        complementary = "<span class='seqFont'>3' - " + leftcSeq + "</span>" + middleDots;
    }
    else {
        forward = "<pre><span class='seqFont'>5' - " + leftfSeq + "</span>" + middleDots;
        complementary = "<span class='seqFont'>3' - " + leftSpace + leftcSeq + "</span>" + middleDots;
    }


    if (overhangs[1] > 0) {
        forward = forward + "<span class='seqFont'>" + rightfSeq + rightSpace + " - 3'</span>";
        complementary = complementary + "<span class='seqFont'>" + rightcSeq + " - 5'</span></pre>";
    }
    else {
        forward = forward + "<span class='seqFont'>" + rightfSeq + " - 3'</span>";
        complementary = complementary + "<span class='seqFont'>" + rightcSeq + rightSpace + " - 5'</span></pre>";
    }

    html = forward + "<br/>" + complementary;
    $("#" + id).html(html);
}

function genSpace(num) {
    var space = "";
    for (var i = 0; i < num; i++) {
        space = space + " ";
    }
    return space;
}