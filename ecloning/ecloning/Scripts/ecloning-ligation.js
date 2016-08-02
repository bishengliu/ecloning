//f1Id and f2Id are the table id of fragments
function show_ligation(f1Id, f2Id) {
    //get the 1st fragment
    var fragment1 = $.grep(FragmentMap, function (d, i) {
        return +d.id === +f1Id;
    })
    //get the 2nd fragment
    var fragment2 = $.grep(FragmentMap, function (d, i) {
        return +d.id === +f2Id;
    })
    console.log(fragment1);
    console.log(fragment2);
    //assume fragment 1 is smaller and fragment 2 is bigger
    //decide which method the ligation can use
    if (fragment1[0].overhangs[0] == 0 && fragment1[0].overhangs[1] == 0 && fragment2[0].overhangs[0] == 0 && fragment2[0].overhangs[1] == 0) {
        //both end are blunt ends
        //ligation directly, 
        //can be clockwise and anticlockwise
        drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 1); //1: direction, both colockwise and anticolockwise
        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
    }

    if (fragment1[0].overhangs[0] != 0 && fragment1[0].overhangs[1] != 0 && fragment2[0].overhangs[0] != 0 && fragment2[0].overhangs[1] != 0) {
        //both are stick ends
        //can have 2 methods
        //if matched, then directly ligated
        //not, then first extended before ligation

        //check whether the overhangs of the 2 fragments match or not
        //if not matched, then must be first extended before ligation
        if (($.inArray(fragment2[0].overhangs[0], fragment1[0].overhangs) != -1 || $.inArray(-fragment2[0].overhangs[0], fragment1[0].overhangs) != -1) && ($.inArray(fragment2[0].overhangs[1], fragment1[0].overhangs) != -1 || $.inArray(-fragment2[0].overhangs[1], fragment1[0].overhangs) != -1)) {
            //the overganges can be matched, but not sure yet, both ends match
            //fragment2 right end overhang >0
            if (fragment2[0].overhangs[1] > 0) {
                //fragment2 right matches with fragment1 left
                //1: direction, colockwise of fragment1
                if (fragment1[0].overhangs[0] < 0
                        &&
                    Math.abs(fragment2[0].overhangs[1]) === Math.abs(fragment1[0].overhangs[0])
                        &&
                    gencomSeq(fragment2[0].cSeq.substring(cSeq.length - fragment2[0].overhangs[1])) === fragment1[0].fSeq.substring(0, Math.abs(fragment1[0].overhangs[0])).toUpperCase())
                {
                    //check whether fragment2 left matches with fragment1 right
                    if (fragment2[0].overhangs[0] + fragment1[0].overhangs[1] === 0) {
                        if (fragment2[0].overhangs[0] < 0
                                &&
                           fragment2[0].fSeq.substring(0, -fragment2[0].overhangs[0]) === gencomSeq(fragment1[0].cSeq.substring(cSeq.length - fragment1[0].overhangs[1])))
                        {
                            //both end match
                            drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                            drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                        }
                        else if (fragment2[0].overhangs[0] > 0
                                &&
                                gencomSeq(fragment2[0].cSeq.substring(0, fragment2[0].overhangs[0])) === fragment1[0].fSeq.substring(fSeq.length + fragment1[0].overhangs[1]))
                        {
                            //both ends match
                            drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                            drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                        }
                        else {
                            drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                        }
                    }
                    else {
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                    }

                }
                //fragment2 right matches with fragment1 right
                else if (fragment1[0].overhangs[1] > 0
                        && 
                    Math.abs(fragment2[0].overhangs[1]) === Math.abs(fragment1[0].overhangs[1])
                        &&
                    gencomSeq(fragment2[0].cSeq.substring(fragment2[0].cSeq.length - fragment2[0].overhangs[1])) === genRevSeq(fragment1[0].cSeq.substring(cSeq.length - fragment1[0].overhangs[1])))
                {
                    //check whether fragment2 left matches with fragment1 left
                    if (fragment2[0].overhangs[0] === fragment1[0].overhangs[0]) {
                        if (fragment2[0].overhangs[0] < 0
                                &&
                            fragment2[0].fSeq.substring(0, -fragment2[0].overhangs[0]).toUpperCase() === gencomSeq(genRevSeq(fragment1[0].fSeq.substring(0, fragment1[0].overhangs[0]))))
                        {
                            //both end match
                            drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                            drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                        }
                        else if (fragment2[0].overhangs[0] > 0
                                    &&
                                fragment2[0].cSeq.substring(0, fragment2[0].overhangs[0]).toUpperCase() === gencomSeq(genRevSeq(fragment1[0].cSeq.substring(0, fragment1[0].overhangs[0]))))
                        {
                            //both end match
                            drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                            drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                        }
                        else {
                            drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                        }
                    }
                    else {
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                    }

                }
                else {
                    //if match not found, need first ligation
                    drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                }                
            }
            else {
                //fragment2 right overhangs < 0

                //fragment2 right matches with fragment1 left
                if (fragment1[0].overhangs[0] > 0
                        && 
                    Math.abs(fragment2[0].overhangs[1]) === Math.abs(fragment1[0].overhangs[0])
                        &&
                    fragment2[0].fSeq.substring(fragment2[0].fSeq.length + fragment2[0].overhangs[1]).toUpperCase() === gencomSeq(fragment1[0].cSeq.substring(0, fragment1[0].overhangs[0])))
                {
                    //check whether fragment left end match with fragment 1 right end
                    if (fragment2[0].overhangs[0] + fragment1[0].overhangs[1] === 0) {
                        if (fragment2[0].overhangs[0] < 0
                                &&
                           fragment2[0].fSeq.substring(0, -fragment2[0].overhangs[0]) === gencomSeq(fragment1[0].cSeq.substring(cSeq.length - fragment1[0].overhangs[1]))) {
                            //both end match
                            drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                            drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                        }
                        else if (fragment2[0].overhangs[0] > 0
                                &&
                                gencomSeq(fragment2[0].cSeq.substring(0, fragment2[0].overhangs[0])) === fragment1[0].fSeq.substring(fSeq.length + fragment1[0].overhangs[1])) {
                            //both ends match
                            drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                            drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                        }
                        else {
                            drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                        }
                    }
                    else {
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                    }

                }
                //fragment2 right end matches with fragment1 right end
                else if (fragment1[0].overhangs[1] < 0
                            && 
                        Math.abs(fragment2[0].overhangs[1]) === Math.abs(fragment1[0].overhangs[1])
                            &&
                        fragment2[0].fSeq.substring(fragment2[0].fSeq.length + fragment2[0].overhangs[1]).toUpperCase() === genRevSeq(fragment1[0].fSeq.substring(fragment1[0].fSeq.length + fragment1[0].overhangs[1])))
                {
                    //check whether fragment2 left end matches with fragment 1 left end
                    if (fragment2[0].overhangs[0] === fragment1[0].overhangs[0]) {
                        if (fragment2[0].overhangs[0] < 0
                                &&
                            fragment2[0].fSeq.substring(0, -fragment2[0].overhangs[0]).toUpperCase() === gencomSeq(genRevSeq(fragment1[0].fSeq.substring(0, fragment1[0].overhangs[0])))) {
                            //both end match
                            drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                            drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                        }
                        else if (fragment2[0].overhangs[0] > 0
                                    &&
                                fragment2[0].cSeq.substring(0, fragment2[0].overhangs[0]).toUpperCase() === gencomSeq(genRevSeq(fragment1[0].cSeq.substring(0, fragment1[0].overhangs[0])))) {
                            //both end match
                            drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                            drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                        }
                        else {
                            drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                        }
                    }
                    else {
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                    }
                }
                else {
                    //if match not found, need first ligation
                    drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                }
            }
        }
        else {
            //if the overhangs don't match, it must be first extended before ligation
            drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
        }
    }

    if (((fragment1[0].overhangs[0] == 0 && fragment1[0].overhangs[1] != 0) || (fragment1[0].overhangs[0] != 0 && fragment1[0].overhangs[1] == 0)) && ((fragment2[0].overhangs[0] == 0 && fragment2[0].overhangs[1] != 0) || (fragment2[0].overhangs[0] != 0 && fragment2[0].overhangs[1] == 0))) {
        //fragment1 and 2: one stick-end and one blunt-end
        //if matched, then directly ligated
        //not, then first extension the ligation

        //fragment2 right end is blunt and left end not
        if (fragment2[0].overhangs[1] == 0 && fragment2[0].overhangs[0] != 0)
        {
            //fragment 2 left end match with fragment1 left end
            if (fragment1[0].overhangs[0] == 0 && Math.abs(fragment1[0].overhangs[1]) == Math.abs(fragment2[0].overhangs[0])) {
                //check whether fragment 2 left matches with fragment1 right end

                //fragment2.overhangs[0] > 0
                if (fragment2[0].overhangs[0] > 0) {
                    if (fragment1[0].overhangs[1] < 0 && fragment1[0].fSeq.substring(fragment1[0].fSeq.length + fragment1[0].overhangs[1]).toUpperCase() === gencomSeq(fragment2[0].cSeq.substring(0, fragment2[0].overhangs[0]))) {
                        //fragment 2 left matches with fragment1 right end
                        drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                    else {
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
                //fragment2.overhangs[0] < 0
                else {
                    if (fragment1[0].overhangs[1] > 0 && gencomSeq(fragment1[0].cSeq.substring(fragment1[0].fSeq.length - fragment1[0].overhangs[1])) === fragment2[0].fSeq.substring(0, -fragment2[0].overhangs[0]).toUpperCase()) {
                        //fragment 2 left matches with fragment1 right end
                        drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                    else {
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
            }
           //fragment 2 left end match with fragment1 right end
            else if (fragment1[0].overhangs[1] == 0 && Math.abs(fragment1[0].overhangs[0]) == Math.abs(fragment2[0].overhangs[0])) {
                //fragment2.overhangs[0] > 0
                if (fragment2[0].overhangs[0] > 0)
                {
                    if (fragment1[0].overhangs[0] > 0 && genRevSeq(fragment1[0].cSeq.substring(0, fragment1[0].overhangs[0])) == gencomSeq(fragment2[0].cSeq.substring(0, fragment2[0].overhangs[0]))) {
                        //fragment2 left matches with fragment 1 left
                        drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                    else {
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
                //fragment2.overhangs[0] < 0
                else
                {
                    if (fragment1[0].overhangs[0] < 0 && gencomSeq(genRevSeq(fragment1[0].fSeq.substring(0, -fragment1[0].overhangs[0]))) == fragment2[0].fSeq.substring(0, -fragment2[0].overhangs[0]).toUpperCase()) {
                        //fragment2 left matches with fragment 1 left
                        drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                    else {
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
            }
            else {
                drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
            }
        }
        //fragment2 left end is blunt and right end not
        else
        {
            //fragment2 right matched fragment1 left
            if (fragment1[0].overhangs[1] == 0 && Math.abs(fragment1[0].overhangs[0]) === Math.abs(fragment2[0].overhangs[1])) {
                //fragment2.overhangs[1] > 0
                if (fragment2[0].overhangs[1] > 0) {
                    if (fragment1[0].overhangs[0] < 0 && fragment1[0].fSeq.substring(0, -fragment1[0].overhangs[0]).toUpperCase() === gencomSeq(fragment2[0].cSeq.substring(fragment2[0].cSeq.length - fragment2[0].overhangs[1]))) {
                        drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                    else {
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
                //fragment2.overhangs[1] < 0
                else
                {
                    if (fragment1[0].overhangs[0] > 0 && gencomSeq(fragment1[0].cSeq.substring(0, fragment1[0].overhangs[0]).toUpperCase()) === fragment2[0].fSeq.substring(fragment2[0].fSeq.length + fragment2[0].overhangs[1]).toUpperCase()) {
                        drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                    else {
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
            }
            //fragment2 right matched fragment1 right
            else if (fragment1[0].overhangs[0] == 0 && Math.abs(fragment1[0].overhangs[1]) === Math.abs(fragment2[0].overhangs[1]))
            {
                //fragment2.overhangs[1] > 0
                if (fragment2[0].overhangs[1] > 0) {
                    if (fragment1[0].overhangs[1] > 0 && genRevSeq(fragment1[0].cSeq.substring(fragment1[0].cSeq.length - fragment1[0].overhangs[1])) === gencomSeq(fragment2[0].cSeq.substring(fragment2[0].cSeq.length - fragment2[0].overhangs[1]))) {
                        drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 3); //2: direction, anticolockwise of fragment1
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                    else {
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
                //fragment2.overhangs[1] < 0
                else {
                    if (fragment1[0].overhangs[1] < 0 && gencomSeq(genRevSeq(fragment1[0].fSeq.substring(fragment1[0].fSeq.length + fragment1[0].overhangs[1]))) === fragment2[0].fSeq.substring(fragment2[0].fSeq.length + fragment2[0].overhangs[1]).toUpperCase()) {
                        drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 3); //2: direction, anticolockwise of fragment1
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                    else {
                        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
            }
            else {
                drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
            }
        }
    }

    if (((fragment1[0].overhangs[0] == 0 && fragment1[0].overhangs[1] != 0) || (fragment1[0].overhangs[0] != 0 && fragment1[0].overhangs[1] == 0)) && (fragment2[0].overhangs[0] != 0 && fragment2[0].overhangs[1] != 0)) {
        //fragment1: one stick-end and one blunt-end
        //fragment2: 2 stick-ends
        //can only be extended first and then ligate
        //can be clockwise and anticlockwise
        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
    }
    if (((fragment2[0].overhangs[0] == 0 && fragment2[0].overhangs[1] != 0) || (fragment2[0].overhangs[0] != 0 && fragment2[0].overhangs[1] == 0)) && (fragment1[0].overhangs[0] != 0 && fragment1[0].overhangs[1] != 0)) {
        //fragment2: one stick-end and one blunt-end
        //fragment1: 2 stick-ends
        //can only be extended first and then ligate
        //can be clockwise and anticlockwise
        drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
    }

}

function gencomSeq(seq) {
    var seqArray = seq.toUpperCase().split('');
    var cseqArray = [];
    $.each(seqArray, function (i, d) {
        var letter;
        if (d == "A") {
            letter = "T";
        } else if (d == "T") {
            letter = "A";
        } else if (d == "G") {
            letter = "C";
        } else {
            letter = "G";
        }
        cseqArray.push(letter);
    })
    return cseqArray.join('');
}

function genRevSeq(seq) {
    var seqArray = seq.toUpperCase().split('');
    var rseqArray = seqArray.reverse();
    return rseqArray.join('');
}

//"method2-cw-map", "method2-acw-map"
function drawInDirectLigation(fragment1, fragment2, idClock, idAntiClock, direction) {
    //indirect ligation always has 2 direction
    console.log("I am trying to draw the indirect ligation for you...")
    var plasmidName = $("#plasmidName").val();
    var plasmidLength = Math.max(fragment1.cSeq.length, fragment1.fSeq.length) + Math.max(fragment2.cSeq.length, fragment2.fSeq.length);
    var showlabels = false;

    //fragment2 properties
    var f2LeftExtension = 3;
    if (fragment2.overhangs[0] < 0) {
        f2LeftExtension = 1;
    } else if (fragment2.overhangs[0] > 0) {
        f2LeftExtension = 2;
    } else {
        f2LeftExtension = 3;
    }

    var f2RightExtension = 3;
    if (fragment2.overhangs[1] < 0) {
        f2RightExtension = 1;
    } else if (fragment2.overhangs[1] > 0) {
        f2RightExtension = 2;
    } else {
        f2RightExtension = 3;
    }

    var f2LeftNum = Math.abs(fragment2.overhangs[0]);
    var f2RightNum = Math.abs(fragment2.overhangs[1]);

    //clockwise
    var frangment1Direction = true;
    if (frangment1Direction) {
        //fragment1 properties
        var f1LeftExtension = 3;
        if (fragment1.overhangs[0] < 0) {
            f1LeftExtension = 1;
        } else if (fragment1.overhangs[0] > 0) {
            f1LeftExtension = 2;
        } else {
            f1LeftExtension = 3;
        }

        var f1RightExtension = 3;
        if (fragment1.overhangs[1] < 0) {
            f1RightExtension = 1;
        } else if (fragment1.overhangs[1] > 0) {
            f1RightExtension = 2;
        } else {
            f1RightExtension = 3;
        }

        var f1LeftNum = Math.abs(fragment1.overhangs[0]);
        var f1RightNum = Math.abs(fragment1.overhangs[1]);

        //generate seq
        //left 104, right 23
        var plasmidfSeq = genIndrectfSeq(fragment1, fragment2, true);
        console.log(plasmidfSeq);
        var plasmidcSeq = genIndrectcSeq(plasmidfSeq);
        console.log(plasmidcSeq);
        var html = InDirectRenderXML(plasmidName, plasmidLength, showlabels, 'clockwise', plasmidfSeq, plasmidcSeq, f1RightExtension, f1RightNum, f1LeftExtension, f1LeftNum, f2RightExtension, f2RightNum, f2LeftExtension, f2LeftNum);
        console.log(html);
        debugger;
        $('#' + idClock).text(html);
    }
    


    //anticlockwise
    frangment1Direction = false;
    if (!frangment1Direction) {
        //fragment1 properties
        var f1LeftExtension = 3;
        if (fragment1.overhangs[1] < 0) {
            f1LeftExtension = 2;
        } else if (fragment1.overhangs[1] > 0) {
            f1LeftExtension = 1;
        } else {
            f1LeftExtension = 3;
        }

        var f1RightExtension = 3;
        if (fragment1.overhangs[0] < 0) {
            f1RightExtension = 2;
        } else if (fragment1.overhangs[0] > 0) {
            f1RightExtension = 1;
        } else {
            f1RightExtension = 3;
        }

        var f1LeftNum = Math.abs(fragment1.overhangs[1]);
        var f1RightNum = Math.abs(fragment1.overhangs[0]);

        //generate seq
        //left 104, right 23
        var plasmidfSeq = genIndrectfSeq(fragment1, fragment2, false);
        var plasmidcSeq = genIndrectcSeq(plasmidfSeq);
        var html = InDirectRenderXML(plasmidName, plasmidLength, showlabels, 'anticlockwise', plasmidfSeq, plasmidcSeq, f1RightExtension, f1RightNum, f1LeftExtension, f1LeftNum, f2RightExtension, f2RightNum, f2LeftExtension, f2LeftNum);
        console.log(html);
        debugger;
        $('#' + idAntiClock).text(html);
    }


}

//"method1-cw-map", "method1-acw-map"
function drawDirectLigation(fragment1, fragment2, idClock, idAntiClock, direction) {
    //1: both directions
    //2: clockwise
    //3 antiClockwise
    console.log("I am trying to draw the direct ligation for you...")

}

//first blunting and then ligation
//23 and 104 are important bp: 104 - 23 are fragment1, 23-104 are fragment2
//plasmidLength: totoal length
//showlabels, false or true
//frangment1Direction =="clockwise" || 'anticlockwise'
//f1RightExtension: 1==fSeq, 2==cSeq, 3==none
//f1LeftExtension: 1==fSeq, 2==cSeq, 3==none
//f2RightExtension, f2LeftExtension: 1==fSeq, 2==cSeq, 3==none
function InDirectRenderXML(plasmidName, plasmidLength, showlabels, frangment1Direction, plasmidfSeq, plasmidcSeq, f1RightExtension, f1RightNum, f1LeftExtension, f1LeftNum, f2RightExtension, f2RightNum, f2LeftExtension, f2LeftNum) {
    var html = '';
    html += '<plasmid sequencelength="130" plasmidheight="375" plasmidwidth="375" class="plasmid-font">';
    html += '<plasmidtrack trackstyle="fill:none;stroke:none" radius="110">';

    //plasmid name

    html += '<tracklabel labelstyle="font-weight:700;fill:#369" text="' + plasmidName + '"></tracklabel>';
    html += '<tracklabel labelstyle="font-size:14px;font-weight:300;fill:#036" text="' + plasmidLength + ' bp" vadjust="20"></tracklabel>';

    //if showlabels, false or true
    if (showlabels) {
        html += '<trackscale interval="5" style="stroke:#999" labelclass="smlabel gray" labelvadjust="12" showlabels="1"></trackscale>';
    }
    //show left shadow
    //<!-- left ligation, 104 in middle -->
    var lstart = 104 - f2LeftNum;
    var lend = 104 + f1LeftNum;
    html += '<trackmarker start="' + lstart + '" end="'+ lend +'" class="greenregion" vadjust="-6" wadjust="5"></trackmarker>';
    //<!-- right ligation, 23 in middle, span 7x2 =14-->
    var rstart = 23 - f1RightNum;
    var rend = 23 + f2RightNum;
    html += '<trackmarker start="' + rstart + '" end="' + rend + '" class="greenregion" vadjust="-6" wadjust="5"></trackmarker>';

    //frangment1Direction =="clockwise" || 'anticlockwise'
    //fragment 1
    if (frangment1Direction == "clockwise") {
        html += '<trackmarker start="120" end="10" markerstyle="fill:green" vadjust="50" wadjust="-23" arrowendlength="5" arrowendwidth="0.5">';
        html += '<markerlabel type="path" text="fragment1, clockwise (+)" valign="outer" labelclass="mdlabel green" vadjust="5"></markerlabel>';
        html += '</trackmarker>';
    }
    else {
        html += '<trackmarker start="120" end="10" markerstyle="fill:green" vadjust="50" wadjust="-23" arrowstartlength="5" arrowstartwidth="0.5">';
        html += '<markerlabel type="path" text="fragment1, anticlockwise (-)" valign="outer" labelclass="mdlabel green" vadjust="5"></markerlabel>';
        html += '</trackmarker>';
    }

    //fragment 2
    html += '<trackmarker start="55" end="75" markerstyle="fill:rgba(64,64,192,0.4)" vadjust="50" wadjust="-23" arrowendlength="5" arrowendwidth="0.5">';
    html += '<markerlabel type="path" text="fragment2, clockwise (+)" valign="outer" labelclass="mdlabel blue" vadjust="5"></markerlabel> ';
    html += '</trackmarker>';


    //<!-- nucleotide list and ticks marks between nucleotides-->

    html += '<trackmarker start="0" end="130">';
    html += '<markerlabel type="path" labelstyle="font-size:8px;font-weight:400;kerning:1.1" text="'+plasmidfSeq+'" halign="start" vadjust="5"></markerlabel>';
    html += '<markerlabel type="path" labelstyle="font-size:8px;font-weight:400" text="'+plasmidcSeq+'" halign="start" vadjust="-10"></markerlabel>';
    html += '</trackmarker>';
    html += '<trackscale interval="1" style="stroke:#c99" ticksize="5" vadjust="-18"></trackscale>';


    //<!-- left ligation -->
    html += '<trackmarker start="104" markerstyle="stroke:rgba(255,0,0,1);stroke-width:1px" vadjust="-6" wadjust="5">';
    html += '<markerlabel type="path" text="Ligation Site" class="mdlabel red" valign="outer" vadjust="35"></markerlabel>';
    html += '</trackmarker>';

    //blunting arrow
    //f1 left
    if (f1LeftExtension === 1) {
        //extension on forward sequence
        var end = 104 + f1LeftNum;
        html += '<trackmarker start="104" end="' + end + '" class="blue" vadjust="30" wadjust="-23" arrowstartlength="5" arrowstartwidth="1">';
        html += '<markerlabel type="path" text="f1-blunting" halign="start" vadjust="15" labelclass="smlabel blue"></markerlabel>';
        html += '</trackmarker>';
    }
    if (f1LeftExtension === 2) {
        //extension on reverse sequence
        var end = 104 + f1LeftNum;
        html += '<trackmarker start="104" end="' + end + '" class="blue" vadjust="-13" wadjust="-23" arrowstartlength="5" arrowstartwidth="1">';
        html += '<markerlabel type="path" text="f1-blunting" halign="start" vadjust="-5" labelclass="smlabel blue"></markerlabel>';
        html += '</trackmarker>';
    }
    //f2 left
    if (f2LeftExtension === 1) {
        //extension on forward sequence
        var start = 104 - f2LeftNum;
        html += '<trackmarker start="' + start + '" end="104" class="purple" vadjust="30" wadjust="-23" arrowendlength="5" arrowendwidth="1">';
        html += '<markerlabel type="path" text="f2-blunting" halign="end" vadjust="15" labelclass="smlabel purple"></markerlabel>';
        html += '</trackmarker>';
    }
    if (f2LeftExtension === 2) {
        //extension on reverse sequence
        var start = 104 - f2LeftNum;
        html += '<trackmarker start="' + start + '" end="104" class="purple" vadjust="-13" wadjust="-23" arrowendlength="5" arrowendwidth="1">';
        html += '<markerlabel type="path" text="f2-blunting" halign="end" vadjust="-5" labelclass="smlabel purple"></markerlabel>';
        html += '</trackmarker>';
    }

    //<!-- right ligation -->
    html += '<trackmarker start="23" markerstyle="stroke:rgba(255,0,0,1);stroke-width:1px" vadjust="-6" wadjust="5">';
    html += '<markerlabel type="path" text="Ligation Site" class="mdlabel red" valign="outer" vadjust="35"></markerlabel>';
    html += '</trackmarker>';


    //blunting arraow
    //f1 right
    if (f1RightExtension === 1) {
        //extension on forward sequence
        var start = 23 - f1RightNum;
        html += '<trackmarker start="' + start + '" end="23" class="blue" vadjust="30" wadjust="-23" arrowendlength="5" arrowendwidth="1">';
        html += '<markerlabel type="path" text="f1-blunting" halign="end" vadjust="15" labelclass="smlabel blue"></markerlabel>';
        html += '</trackmarker>';
    }
    if (f1RightExtension === 2) {
        //extension on reverse sequence
        var start = 23 - f1RightNum;
        html += '<trackmarker start="'+start+'" end="23" class="blue" vadjust="-13" wadjust="-23" arrowendlength="5" arrowendwidth="1">';
        html += '<markerlabel type="path" text="f1-blunting" halign="end" vadjust="-5" labelclass="smlabel blue"></markerlabel>';
        html += '</trackmarker>';
    }

    //f2 right
    if (f2RightExtension == 1) {
        //forward
        var end = 23 + f2RightNum;
        html += '<trackmarker start="23" end="'+ end +'" class="purple" vadjust="30" wadjust="-23" arrowstartlength="5" arrowstartwidth="1">';
        html += '<markerlabel type="path" text="f2-blunting" halign="start" vadjust="15" labelclass="smlabel purple"></markerlabel>';
        html += '</trackmarker>';
    }
    if (f2RightExtension == 2) {
        //reverse
        var end = 23 + f2RightNum;
        html += '<trackmarker start="23" end="' + end + '" class="purple" vadjust="-13" wadjust="-23" arrowstartlength="5" arrowstartwidth="1">';
        html += '<markerlabel type="path" text="f2-blunting" halign="start" vadjust="-5" labelclass="smlabel purple"></markerlabel>';
        html += '</trackmarker>';
    }


    html += '</plasmidtrack>';
    html += '</plasmid>';
    return html;
}

//direct ligation
//111 - 16 are fragment1 and 16 - 111 are fragment 2
//plasmidLength: totoal length
//showlabels, false or true
//frangment1Direction =="clockwise" || 'anticlockwise'
//f1RightStick, f1LeftStick and f2RightStick, f2LeftStick: 1==fSeq, 2==cSeq, 3==none
//rightStickLength, LeftStickLength >=0, stick end overhang length
//
function DirectRenderXML(plasmidName, plasmidLength, showlabels, frangment1Direction, plasmidfSeq, plasmidcSeq, f1RightStick, rightStickLength, f2RightStick, f1LeftStick, LeftStickLength, f2LeftStick) {
    var html = '';
    html += '<plasmid sequencelength="130" plasmidheight="375" plasmidwidth="375" class="plasmid-font">';
    html += '<plasmidtrack trackstyle="fill:none;stroke:none" radius="110">';

    //plasmid name

    html += '<tracklabel labelstyle="font-weight:700;fill:#369" text="' + plasmidName + '"></tracklabel>';
    html += '<tracklabel labelstyle="font-size:14px;font-weight:300;fill:#036" text="' + plasmidLength + ' bp" vadjust="20"></tracklabel>';

    //if showlabels, false or true
    if (showlabels) {
        html += '<trackscale interval="5" style="stroke:#999" labelclass="smlabel gray" labelvadjust="12" showlabels="1"></trackscale>';
    }

    //frangment1Direction =="clockwise" || 'anticlockwise'
    //fragment 1
    if (frangment1Direction == "clockwise") {
        html += '<trackmarker start="120" end="10" markerstyle="fill:green" vadjust="50" wadjust="-23" arrowendlength="5" arrowendwidth="0.5">';
        html += '<markerlabel type="path" text="fragment1, clockwise (+)" valign="outer" labelclass="mdlabel green" vadjust="5"></markerlabel>';
        html += '</trackmarker>';
    }
    else {
        html += '<trackmarker start="120" end="10" markerstyle="fill:green" vadjust="50" wadjust="-23" arrowstartlength="5" arrowstartwidth="0.5">';
        html += '<markerlabel type="path" text="fragment1, anticlockwise (-)" valign="outer" labelclass="mdlabel green" vadjust="5"></markerlabel>';
        html += '</trackmarker>';
    }

    //fragment 2
    html += '<trackmarker start="55" end="75" markerstyle="fill:rgba(64,64,192,0.4)" vadjust="50" wadjust="-23" arrowendlength="5" arrowendwidth="0.5">';
    html += '<markerlabel type="path" text="fragment2, clockwise (+)" valign="outer" labelclass="mdlabel blue" vadjust="5"></markerlabel> ';
    html += '</trackmarker>';



    //<!-- nucleotide list and ticks marks between nucleotides-->

    html += '<trackmarker start="0" end="130">';
    html += '<markerlabel type="path" labelstyle="font-size:8px;font-weight:400;kerning:1.1" text="' + plasmidfSeq + '" halign="start" vadjust="5"></markerlabel>';
    html += '<markerlabel type="path" labelstyle="font-size:8px;font-weight:400" text="' + plasmidcSeq + '" halign="start" vadjust="-10"></markerlabel>';
    html += '</trackmarker>';
    html += '<trackscale interval="1" style="stroke:#c99" ticksize="5" vadjust="-18"></trackscale>';



    //<!-- left ligation, 111 sep -->
    if (f1LeftStick == 1) {
        //fSeq
        var start = 111 - LeftStickLength;
        html += '<trackmarker start="'+start+'" end="111" class="greenregion" vadjust="9" wadjust="-10"></trackmarker>';
    }
    if (f1LeftStick == 2) {
        //cSeq
        var start = 111 - LeftStickLength;
        html += '<trackmarker start="' + start + '" end="111" class="greenregion" vadjust="-6" wadjust="-10"></trackmarker>';

    }
    if (f2LeftStick == 2) {
        //cSeq
        var start = 111 - LeftStickLength;
        html += '<trackmarker start="' + start + '" end="111" class="purpleregion" vadjust="-6" wadjust="-10"></trackmarker>';
    }
    if (f2LeftStick == 1) {
        //fSeq
        var start = 111 - LeftStickLength;
        html += '<trackmarker start="' + start + '" end="111" class="purpleregion" vadjust="9" wadjust="-10"></trackmarker>';
    }
    if (LeftStickLength === 0) {
        html += '<trackmarker start="111" markerstyle="stroke:gray;stroke-width:1px" vadjust="-6" wadjust="5">';
        html += '<markerlabel type="path" text="Ligation Site" class="mdlabel red" valign="outer" vadjust="25"></markerlabel>';
        html += '</trackmarker>';
    }

    //right ligation, 16 sep
    if (f1RightStick == 1) {
        //fSeq
        var end = 16 + rightStickLength;
        html += '<trackmarker start="16" end="' + end + '" class="greenregion" vadjust="9" wadjust="-10"></trackmarker>';
    }
    if (f1RightStick == 2) {
        //cSeq
        var end = 16 + rightStickLength;
        html += '<trackmarker start="16" end="' + end + '" class="greenregion" vadjust="-6" wadjust="-10"></trackmarker>';
    }
    if (f2RightStick == 1) {
        //fSeq
        var end = 16 + rightStickLength;
        html += '<trackmarker start="16" end="' + end + '" class="purpleregion" vadjust="9" wadjust="-10"></trackmarker>';
    }
    if (f2RightStick == 2) {
        //cSeq
        var end = 16 + rightStickLength;
        html += '<trackmarker start="16" end="' + end + '" class="purpleregion" vadjust="-6" wadjust="-10"></trackmarker>';
    }
    if (rightStickLength === 0) {
        html += '<trackmarker start="16" markerstyle="stroke:gray;stroke-width:1px" vadjust="-6" wadjust="5">';
        html += '<markerlabel type="path" text="Ligation Site" class="mdlabel red" valign="outer" vadjust="25"></markerlabel>';
        html += '</trackmarker>';
    }


    //<!-- middle shadow-->
    //f1
    html += '<trackmarker start="111" end="16" class="greenregion" vadjust="-6" wadjust="5"></trackmarker>';
    //f2
    var mstart = 16 + rightStickLength;
    var mend = 111 - LeftStickLength;
    html += '<trackmarker start="' + mstart + '" end="' + mend + '" class="purpleregion" vadjust="-6" wadjust="5"></trackmarker>';



    html += '</plasmidtrack>';
    html += '</plasmid>';
    return html;
}


function genIndrectfSeq(fragment1, fragment2, clockwise) {
    var f1fSeq = '';
    var f2fSeq = fragment2.fSeq;
    if (clockwise) {
        //generate fSeq
        f1fSeq = fragment1.fSeq;
        //process right of fragment1
        if (fragment1.overhangs[1] < 0) {
            var f1RightBlunting = gencomSeq(fragment1.cSeq.substring(fragment1.cSeq.length + fragment1.overhangs[1]));
            f1fSeq = f1fSeq + f1RightBlunting;
        }
        //left of fragment1
        if (fragment1.overhangs[0] > 0) {
            var f1LeftBlunting = gencomSeq(fragment1.cSeq.substring(0, fragment1.overhangs[0]));
            f1fSeq = f1LeftBlunting + f1fSeq;
        }
    }
    else
    {
        //anticlock
        f1fSeq =genRevSeq(fragment1.fSeq);
        //process right of fragment1
        if (fragment1.overhangs[0] > 0) {
            var f1RightBlunting = gencomSeq(genRevSeq(fragment1.cSeq).substring(fragment1.cSeq.length - fragment1.overhangs[0]));
            f1fSeq = f1fSeq + f1RightBlunting;
        }
        //left of fragment1
        if (fragment1.overhangs[1] > 0) {
            var f1LeftBlunting = gencomSeq(genRevSeq(fragment1.cSeq).substring(0, fragment1.overhangs[0]));
            f1fSeq = f1LeftBlunting + f1fSeq;
        }
    }

        //process right of fragment2
        if (fragment2.overhangs[0] > 0) {
            var f2RightBlunting = gencomSeq(fragment2.cSeq.substring(0, fragment2.overhangs[0]));
            f2fSeq = f2RightBlunting + f2fSeq;
        }
        //left of fragment 2
        if (fragment2.overhangs[1] > 0) {
            var f2LeftBlunting = gencomSeq(fragment2.cSeq.substring(fragment2.cSeq.length - fragment2.overhangs[1]));
            f2fSeq = f2fSeq + f2LeftBlunting;
        }


        //fragment 104 - 23 // total 130
        var result = '';
        f1fSeqFront = '';
        f1fSeqEnd = '';
        f2fSeqFront = '';
        f2fSeqEnd = '';

        var f1MinLen = 130-104 +1 + 23;
        if (f1fSeq.length == f1MinLen) {
            f1fSeqFront = f1fSeq.substring(f1fSeq.length - 23);
            f1fSeqEnd = f1fSeq.substring(0, 27);
        }
        else if (f1fSeq.length > f1MinLen) {
            f1fSeqFront = '...' + f1fSeq.substring(f1fSeq.length - 26);
            f1fSeqEnd = f1fSeq.substring(0, 24) + '...';
        }
        else {
            //too short
            var length = Math.floor(f1fSeq.length / 2);
            f1fSeqFront = genDot(f1MinLen - length) + f1fSeq.substring(f1fSeq.length - length);
            f1fSeqEnd = f1fSeq.substring(0, f1fSeq.length - length) + genDot(f1MinLen - length);
        }
        //64 and 66
        var f2MinLen = 104 - 23 + 1;
        if (f2fSeq.length == f2MinLen) {
            result = f1fSeqFront + f2fSeq + f1fSeqEnd;
        }
        else if (f2fSeq.length > f2MinLen) {
            f2fSeqFront = f2fSeq.substring(0, 38)+ '...';
            f2fSeqEnd = '...' + f2fSeq.substring(f2fSeq.length - 38)
            result = f1fSeqFront + f2fSeqFront + f2fSeqEnd + f1fSeqEnd;
        }
        else {
            //too short
            var length = Math.floor(f1fSeq.length / 2);
            f2fSeqFront = f2fSeq.substring(0, f2fSeq.length - length) + genDot(f2MinLen - length);
            f2fSeqEnd = genDot(f2MinLen - length) + f2fSeq.substring(f2fSeq.length - length);
            result = f1fSeqFront + f2fSeqFront + f2fSeqEnd + f1fSeqEnd;           
        }
    
        return result;
}


function genIndrectcSeq(fSeq)
{
    var array = fSeq.split('');
    var outArray = [];
    $.each(array, function (i, d) {
        if (d == "A") {
            outArray.push('T');
        }
        else if (d == "T") {
            outArray.push('A');
        }
        else if (d == "G") {
            outArray.push('C');
        }
        else if (d == "C") {
            outArray.push('G');
        } else {
            outArray.push('.');
        }
    })
    return outArray.join('');
}
function genDot(num) {
    var out = "";
    if (num > 0) {
        for (i = 0; i <= num; i++) {
            out += ".";
        }
    }
    return out;
}