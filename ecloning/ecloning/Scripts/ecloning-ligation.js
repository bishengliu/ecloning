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
}

//"method1-cw-map", "method1-acw-map"
function drawDirectLigation(fragment1, fragment2, idClock, idAntiClock, direction) {
    //1: both directions
    //2: clockwise
    //3 antiClockwise
    console.log("I am trying to draw the direct ligation for you...")

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


function genLigationcSeq(fSeq)
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
function genDash(num) {
    var out = "";
    if (num > 0) {
        for (i = 0; i <= num; i++) {
            out += "-";
        }
    }
    return out;
}



//draw ligation plasmid
function DrawPlasmid(fSeqArray, cSeqArray, clockwise, fBluntingArray, cBluntingArray, plasmidName, plasmidLength, id) {
    //check the length of the seq
    var fSeq = fSeqArray[0] + fSeqArray[1] + fSeqArray[2];
    var cSeq = cSeqArray[0] + cSeqArray[1] + cSeqArray[2];


    if (fSeq.length != 120 || cSeq.length != 120) {
        console.log('invalid sequence! Sequence must contain 120bp!');
    }
    else {
        //generate array of objects
        var space = 360 / 120;
        var data1 = genArray(fSeq, fSeqArray, fBluntingArray, space);
        var data2 = genArray(cSeq, cSeqArray, cBluntingArray, space);
        //draw canvas
        var margin = { top: 20, right: 20, bottom: 20, left: 20 },
        width = 500 - margin.left - margin.right,
        height = width;

        var radius = width / 2, step = 20;


        //forward seq
        var farc = d3.svg.arc()
            .outerRadius(radius - step)
            .innerRadius(radius - step * 2);
        var fpie = d3.layout.pie()
                .sort(null)
                .value(function (d) { return d.space; });
        //complement seq
        var carc = d3.svg.arc()
            .outerRadius(radius - step * 2)
            .innerRadius(radius - step * 3);
        var cpie = d3.layout.pie()
                .sort(null)
                .value(function (d) { return d.space; });
        //symbol

        var s1arc = d3.svg.arc()
            .outerRadius(radius - step * 2.3)
            .innerRadius(radius - step * 2.1);
        var s2arc = d3.svg.arc()
            .outerRadius(radius - step * 1.9)
            .innerRadius(radius - step * 1.7);

        //draw arrow
        var f1arc;
        if (clockwise) {
            f1arc = d3.svg.arc()
                        .innerRadius(radius - 4 * step - 2)
                        .outerRadius(radius - 4 * step)
                        .startAngle(0)
                        .endAngle(-2 * Math.PI * 1 / 6);
        }
        else {
            f1arc = d3.svg.arc()
                        .innerRadius(radius - 4 * step - 2)
                        .outerRadius(radius - 4 * step)
                        .startAngle(-2 * Math.PI * 1 / 6)
                        .endAngle(0);
        }

        var f2arc = d3.svg.arc()
                        .innerRadius(radius - 4 * step - 2)
                        .outerRadius(radius - 4 * step)
                        .startAngle(2 * Math.PI * 3 / 6)
                        .endAngle(2 * Math.PI * 2 / 6);


        //draw svg container
        var svg = d3.select(id).append("svg")
                    .attr("width", width)
                    .attr("height", height)
                  .append("g")
                    .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");

        svg.append("text")
              .attr("dy", ".35em")
              .attr("text-anchor", "middle")
              .attr("class", "pName")
              .text(plasmidName);

        svg.append("text")
              .attr("y", 25)
              .attr("dy", ".35em")
              .attr("text-anchor", "middle")
              .text(plasmidLength + " bp");

        //forward
        var fg = svg.selectAll(".farc")
              .data(fpie(data1))
            .enter().append("g")
              .attr("class", "farc");


        fg.append("path")
            .attr("d", farc)
            .style("stroke", function (d) { return d.data.fragment == "f1" ? "rgba(192,255,192,0.2)" : "rgba(192,192,255,0.2)" })
            .style("fill", function (d) { return d.data.fragment == "f1" ? "rgba(64,192,64,0.2)" : "rgba(64,64,192,0.2)" });

        fg.append("text")
              .attr("transform", function (d) { return "translate(" + farc.centroid(d) + ")rotate(" + angle(d) + ")"; })
              .attr("dy", ".35em")
              .attr("fill", function (d) { return d.data.blunting ? "red" : "black" })
              .text(function (d) { return d.data.letter; });

        //complement
        var cg = svg.selectAll(".carc")
              .data(cpie(data2))
            .enter().append("g")
              .attr("class", "carc");


        cg.append("path")
            .attr("d", carc)
            .style("stroke", function (d) { return d.data.fragment == "f1" ? "rgba(192,255,192,0.2)" : "rgba(192,192,255,0.2)" })
            .style("fill", function (d) { return d.data.fragment == "f1" ? "rgba(64,192,64,0.2)" : "rgba(64,64,192,0.2)" });

        cg.append("text")
              .attr("transform", function (d) { return "translate(" + carc.centroid(d) + ")rotate(" + angle(d) + ")"; })
              .attr("dy", ".35em")
              .attr("fill", function (d) { return d.data.blunting ? "red" : "black" })
              .text(function (d) { return d.data.letter; });

        cg.append("polyline")
             .attr("points", function (d) {
                 return [s1arc.centroid(d), s2arc.centroid(d)];
             })
            .attr("stroke", function (d) {
                return "gray";
            });


        svg.append("text")
                .attr("y", -120)
                .attr("dy", ".35em")
                .attr("text-anchor", "middle")
                .style("stroke", "rgba(192,255,192,0.2)")
                .text(function () { return clockwise ? "fragment1 (+)" : "fragment1 (-)"; });

        svg.append("text")
                 .attr("y", 120)
                 .attr("dy", ".35em")
                 .attr("text-anchor", "middle")
                 .style("stroke", "rgba(192,192,255,0.2)")
                 .text("fragment2 (+)");


        //f1 arrow
        var f1Arrow = svg.append("svg:defs").append("svg:marker")
            .attr("id", "f1_triangle")
            .attr("refX", 6)
            .attr("refY", function () { return clockwise ? 4 : 8; })
            .attr("markerWidth", 30)
            .attr("markerHeight", 30)
            .attr("orient", function () { return clockwise ? "0deg" : "110deg"; })
            .append("path")
            .attr("d", "M 0 0 12 6 0 12 3 6")
            .style("fill", "rgba(64,192,64,1)");

        svg.append("path")
                        .attr("d", f1arc)
                        .attr("stroke-width", 1)
                        .attr("stroke", "black")
                        .attr("marker-end", "url(#f1_triangle)")
                          .style("stroke", "rgba(192,255,192,0.2)")
                          .style("fill", "rgba(64,192,64,1)")
                          .attr("transform", function (d) { return "rotate(" + 30 + ")"; });

        //f2 arrow
        var f2Arrow = svg.append("svg:defs").append("svg:marker")
            .attr("id", "f2_triangle")
            .attr("refX", 6)
            .attr("refY", 5)
            .attr("markerWidth", 30)
            .attr("markerHeight", 30)
            .attr("orient", "180deg")
            .append("path")
            .attr("d", "M 0 0 12 6 0 12 3 6")
            .style("fill", "rgba(192,192,255,1)");

        svg.append("path")
                        .attr("d", f2arc)
                        .attr("stroke-width", 1)
                        .attr("stroke", "black")
                        .attr("marker-end", "url(#f2_triangle)")
                          .style("stroke", "rgba(192,255,192,0.2)")
                          .style("fill", "rgba(192,192,255,1)")
                          .attr("transform", function (d) { return "rotate(" + 30 + ")"; });
    }

}

function genArray(seq, array, bluntingArray, space) {
    //process forwar seq
    var letterArray = seq.split('');
    var data = [];
    $.each(letterArray, function (i, d) {
        var obj = {};
        obj.blunting = (bluntingArray.length > 0 && $.inArray(i, bluntingArray) != -1) ? true : false;
        obj.fragment = (i <= array[0].length || i >= (120 - array[2].length)) ? "f1" : "f2";
        obj.id = i;
        obj.letter = d.toUpperCase();
        obj.space = space;
        data.push(obj);
    })
    return data;
}

function angle(d) {
    var a = (d.startAngle + d.endAngle) * 90 / Math.PI + 180;
    return a > 90 ? a - 180 : a;
}