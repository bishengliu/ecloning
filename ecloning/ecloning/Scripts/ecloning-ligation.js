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
    if (fragment1.overhangs[0] == 0 && fragment1.overhangs[1] == 0 && fragment2.overhangs[0] == 0 && fragment2.overhangs[1] == 0) {
        //both end are blunt ends
        //ligation directly, 
        //can be clockwise and anticlockwise
        drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 1); //1: direction, both colockwise and anticolockwise
    }

    if (fragment1.overhangs[0] != 0 && fragment1.overhangs[1] != 0 && fragment2.overhangs[0] != 0 && fragment2.overhangs[1] != 0) {
        //both are stick ends
        //can have 2 methods
        //if matched, then directly ligated
        //not, then first extended before ligation

        //check whether the overhangs of the 2 fragments match or not
        //if not matched, then must be first extended before ligation
        if (($.inArray(fragment2.overhangs[0], fragment1.overhangs) != -1 || $.inArray(-fragment2.overhangs[0], fragment1.overhangs) != -1) && ($.inArray(fragment2.overhangs[1], fragment1.overhangs) != -1 || $.inArray(-fragment2.overhangs[1], fragment1.overhangs) != -1)) {
            //the overganges can be matched, but not sure yet, both ends match
            //fragment2 right end overhang >0
            if (fragment2.overhangs[1] > 0) {
                //fragment2 right matches with fragment1 left
                //1: direction, colockwise of fragment1
                if (fragment1.overhangs[0] < 0
                        &&
                    Math.abs(fragment2.overhangs[1]) === Math.abs(fragment1.overhangs[0])
                        &&
                    gencomSeq(fragment2.cSeq.substring(cSeq.length - fragment2.overhangs[1])) === fragment1.fSeq.substring(0, Math.abs(fragment1.overhangs[0])).toUpperCase())
                {
                    //check whether fragment2 left matches with fragment1 right
                    if (fragment2.overhangs[0] + fragment1.overhangs[1] === 0) {
                        if(fragment2.overhangs[0] < 0
                                &&
                           fragment2.fSeq.substring(0, -fragment2.overhangs[0]) === gencomSeq(fragment1.cSeq.substring(cSeq.length - fragment1.overhangs[1])))
                        {
                            //both end match
                            drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                        }
                        else if (fragment2.overhangs[0] > 0
                                &&
                                gencomSeq(fragment2.cSeq.substring(0, fragment2.overhangs[0])) === fragment1.fSeq.substring(fSeq.length + fragment1.overhangs[1]))
                        {
                            //both ends match
                            drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                        }
                        else {
                            drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                        }
                    }
                    else {
                        drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                    }

                }
                //fragment2 right matches with fragment1 right
                else if (fragment1.overhangs[1] > 0
                        && 
                    Math.abs(fragment2.overhangs[1]) === Math.abs(fragment1.overhangs[1])
                        &&
                    gencomSeq(fragment2.cSeq.substring(cSeq.length - fragment2.overhangs[1])) === genRevSeq(fragment1.cSeq.substring(cSeq.length - fragment1.overhangs[1])))
                {
                    //check whether fragment2 left matches with fragment1 left
                    if (fragment2.overhangs[0] === fragment1.overhangs[0]) {
                        if (fragment2.overhangs[0] < 0
                                &&
                            fragment2.fSeq.substring(0, -fragment2.overhangs[0]).toUpperCase() === gencomSeq(genRevSeq(fragment1.fSeq.substring(0, fragment1.overhangs[0]))))
                        {
                            //both end match
                            drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                        }
                        else if (fragment2.overhangs[0] > 0
                                    &&
                                fragment2.cSeq.substring(0, fragment2.overhangs[0]).toUpperCase() === gencomSeq(genRevSeq(fragment1.cSeq.substring(0, fragment1.overhangs[0]))))
                        {
                            //both end match
                            drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                        }
                        else {
                            drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                        }
                    }
                    else {
                        drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                    }

                }
                else {
                    //if match not found, need first ligation
                    drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                }                
            }
            else {
                //fragment2 right overhangs < 0

                //fragment2 right matches with fragment1 left
                if(fragment1.overhangs[0] > 0 
                        && 
                    Math.abs(fragment2.overhangs[1]) === Math.abs(fragment1.overhangs[0])
                        &&
                    fragment2.fSeq.substring(fragment2.fSeq.length + fragment2.overhangs[1]).toUpperCase() === gencomSeq(fragment1.cSeq.substring(0, fragment1.overhangs[0])))
                {
                    //check whether fragment left end match with fragment 1 right end
                    if (fragment2.overhangs[0] + fragment1.overhangs[1] === 0) {
                        if (fragment2.overhangs[0] < 0
                                &&
                           fragment2.fSeq.substring(0, -fragment2.overhangs[0]) === gencomSeq(fragment1.cSeq.substring(cSeq.length - fragment1.overhangs[1]))) {
                            //both end match
                            drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                        }
                        else if (fragment2.overhangs[0] > 0
                                &&
                                gencomSeq(fragment2.cSeq.substring(0, fragment2.overhangs[0])) === fragment1.fSeq.substring(fSeq.length + fragment1.overhangs[1])) {
                            //both ends match
                            drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                        }
                        else {
                            drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                        }
                    }
                    else {
                        drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                    }

                }
                //fragment2 right end matches with fragment1 right end
                else if (fragment1.overhangs[1] < 0
                            && 
                        Math.abs(fragment2.overhangs[1]) === Math.abs(fragment1.overhangs[1])
                            &&
                        fragment2.fSeq.substring(fragment2.fSeq.length + fragment2.overhangs[1]).toUpperCase() === genRevSeq(fragment1.fSeq.substring(fragment1.fSeq.length + fragment1.overhangs[1])))
                {
                    //check whether fragment2 left end matches with fragment 1 left end
                    if (fragment2.overhangs[0] === fragment1.overhangs[0]) {
                        if (fragment2.overhangs[0] < 0
                                &&
                            fragment2.fSeq.substring(0, -fragment2.overhangs[0]).toUpperCase() === gencomSeq(genRevSeq(fragment1.fSeq.substring(0, fragment1.overhangs[0])))) {
                            //both end match
                            drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                        }
                        else if (fragment2.overhangs[0] > 0
                                    &&
                                fragment2.cSeq.substring(0, fragment2.overhangs[0]).toUpperCase() === gencomSeq(genRevSeq(fragment1.cSeq.substring(0, fragment1.overhangs[0])))) {
                            //both end match
                            drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                        }
                        else {
                            drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                        }
                    }
                    else {
                        drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                    }
                }
                else {
                    //if match not found, need first ligation
                    drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
                }
            }
        }
        else {
            //if the overhangs don't match, it must be first extended before ligation
            drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1); //1: direction, both colockwise and anticolockwise
        }
    }

    if (((fragment1.overhangs[0] == 0 && fragment1.overhangs[1] != 0) || (fragment1.overhangs[0] != 0 && fragment1.overhangs[1] == 0)) && ((fragment2.overhangs[0] == 0 && fragment2.overhangs[1] != 0) || (fragment2.overhangs[0] != 0 && fragment2.overhangs[1] == 0))) {
        //fragment1 and 2: one stick-end and one blunt-end
        //if matched, then directly ligated
        //not, then first extension the ligation

        //fragment2 right end is blunt and left end not
        if (fragment2.overhangs[1] == 0 && fragment2.overhangs[0] != 0)
        {
            //fragment 2 left end match with fragment1 left end
            if (fragment1.overhangs[0] == 0 && Math.abs(fragment1.overhangs[1]) == Math.abs(fragment2.overhangs[0])) {
                //check whether fragment 2 left matches with fragment1 right end

                //fragment2.overhangs[0] > 0
                if (fragment2.overhangs[0] > 0) {
                    if (fragment1.overhangs[1] < 0 && fragment1.fSeq.substring(fragment1.fSeq.length + fragment1.overhangs[1]).toUpperCase() === gencomSeq(fragment2.cSeq.substring(0, fragment2.overhangs[0]))) {
                        //fragment 2 left matches with fragment1 right end
                        drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                    }
                    else {
                        drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
                //fragment2.overhangs[0] < 0
                else {
                    if (fragment1.overhangs[1] > 0 && gencomSeq(fragment1.cSeq.substring(fragment1.fSeq.length - fragment1.overhangs[1])) === fragment2.fSeq.substring(0, -fragment2.overhangs[0]).toUpperCase()) {
                        //fragment 2 left matches with fragment1 right end
                        drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                    }
                    else {
                        drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
            }
           //fragment 2 left end match with fragment1 right end
            else if (fragment1.overhangs[1] == 0 && Math.abs(fragment1.overhangs[0]) == Math.abs(fragment2.overhangs[0])) {
                //fragment2.overhangs[0] > 0
                if (fragment2.overhangs[0] > 0)
                {
                    if (fragment1.overhangs[0] > 0 && genRevSeq(fragment1.cSeq.substring(0, fragment1.overhangs[0])) == gencomSeq(fragment2.cSeq.substring(0, fragment2.overhangs[0]))) {
                        //fragment2 left matches with fragment 1 left
                        drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                    }
                    else {
                        drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
                //fragment2.overhangs[0] < 0
                else
                {
                    if (fragment1.overhangs[0] < 0 && gencomSeq(genRevSeq(fragment1.fSeq.substring(0, -fragment1.overhangs[0]))) == fragment2.fSeq.substring(0, -fragment2.overhangs[0]).toUpperCase()) {
                        //fragment2 left matches with fragment 1 left
                        drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                    }
                    else {
                        drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
            }
            else {
                drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
            }
        }
        //fragment2 left end is blunt and right end not
        else
        {
            //fragment2 right matched fragment1 left
            if (fragment1.overhangs[1] == 0 && Math.abs(fragment1.overhangs[0]) === Math.abs(fragment2.overhangs[1])) {
                //fragment2.overhangs[1] > 0
                if (fragment2.overhangs[1] > 0) {
                    if (fragment1.overhangs[0] < 0 && fragment1.fSeq.substring(0, -fragment1.overhangs[0]).toUpperCase() === gencomSeq(fragment2.cSeq.substring(fragment2.cSeq.length - fragment2.overhangs[1]))) {
                        drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                    }
                    else {
                        drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
                //fragment2.overhangs[1] < 0
                else
                {
                    if (fragment1.overhangs[0] > 0 && gencomSeq(fragment1.cSeq.substring(0, fragment1.overhangs[0]).toUpperCase()) === fragment2.fSeq.substring(fragment2.fSeq.length + fragment2.overhangs[1]).toUpperCase()) {
                        drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                    }
                    else {
                        drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
            }
            //fragment2 right matched fragment1 right
            else if (fragment1.overhangs[0] == 0 && Math.abs(fragment1.overhangs[1]) === Math.abs(fragment2.overhangs[1]))
            {
                //fragment2.overhangs[1] > 0
                if (fragment2.overhangs[1] > 0) {
                    if (fragment1.overhangs[1] > 0 && genRevSeq(fragment1.cSeq.substring(fragment1.cSeq.length - fragment1.overhangs[1])) === gencomSeq(fragment2.cSeq.substring(fragment2.cSeq.length - fragment2.overhangs[1]))) {
                        drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 3); //2: direction, anticolockwise of fragment1
                    }
                    else {
                        drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
                //fragment2.overhangs[1] < 0
                else {
                    if (fragment1.overhangs[1] < 0 && gencomSeq(genRevSeq(fragment1.fSeq.substring(fragment1.fSeq.length + fragment1.overhangs[1]))) === fragment2.fSeq.substring(fragment2.fSeq.length + fragment2.overhangs[1]).toUpperCase()) {
                        drawDirectLigation(fragment1, fragment2, "method1-cw-map", "method1-acw-map", 3); //2: direction, anticolockwise of fragment1
                    }
                    else {
                        drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
                    }
                }
            }
            else {
                drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
            }
        }
    }

    if (((fragment1.overhangs[0] == 0 && fragment1.overhangs[1] != 0) || (fragment1.overhangs[0] != 0 && fragment1.overhangs[1] == 0)) && (fragment2.overhangs[0] != 0 && fragment2.overhangs[1] != 0)) {
        //fragment1: one stick-end and one blunt-end
        //fragment2: 2 stick-ends
        //can only be extended first and then ligate
        //can be clockwise and anticlockwise
        drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
    }
    if (((fragment2.overhangs[0] == 0 && fragment2.overhangs[1] != 0) || (fragment2.overhangs[0] != 0 && fragment2.overhangs[1] == 0)) && (fragment1.overhangs[0] != 0 && fragment1.overhangs[1] != 0)) {
        //fragment2: one stick-end and one blunt-end
        //fragment1: 2 stick-ends
        //can only be extended first and then ligate
        //can be clockwise and anticlockwise
        drawInDirectLigation(fragment1, fragment2, "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise
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

}

//"method1-cw-map", "method1-acw-map"
function drawDirectLigation(fragment1, fragment2, idClock, idAntiClock, direction) {
    //1: both directions
    //2: clockwise
    //3 antiClockwise

}