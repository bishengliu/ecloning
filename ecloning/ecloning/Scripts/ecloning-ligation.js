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
                        }
                        else if (fragment2[0].overhangs[0] > 0
                                &&
                                gencomSeq(fragment2[0].cSeq.substring(0, fragment2[0].overhangs[0])) === fragment1[0].fSeq.substring(fSeq.length + fragment1[0].overhangs[1]))
                        {
                            //both ends match
                            drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                        }

                    }

                }
                //fragment2 right matches with fragment1 right
                else if (fragment1[0].overhangs[1] > 0
                        && 
                    Math.abs(fragment2[0].overhangs[1]) === Math.abs(fragment1[0].overhangs[1])
                        &&
                    gencomSeq(fragment2[0].cSeq.substring(fragment2[0].cSeq.length - fragment2[0].overhangs[1])) === genRevSeq(fragment1[0].cSeq.substring(fragment1[0].cSeq.length - fragment1[0].overhangs[1])))
                {
                    //check whether fragment2 left matches with fragment1 left
                    if (fragment2[0].overhangs[0] === fragment1[0].overhangs[0]) {
                        if (fragment2[0].overhangs[0] < 0
                                &&
                            fragment2[0].fSeq.substring(0, -fragment2[0].overhangs[0]).toUpperCase() === gencomSeq(genRevSeq(fragment1[0].fSeq.substring(0, fragment1[0].overhangs[0]))))
                        {
                            //both end match
                            drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                        }
                        else if (fragment2[0].overhangs[0] > 0
                                    &&
                                fragment2[0].cSeq.substring(0, fragment2[0].overhangs[0]).toUpperCase() === gencomSeq(genRevSeq(fragment1[0].cSeq.substring(0, fragment1[0].overhangs[0]))))
                        {
                            //both end match
                            drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                        }
                    }

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
                        }
                        else if (fragment2[0].overhangs[0] > 0
                                &&
                                gencomSeq(fragment2[0].cSeq.substring(0, fragment2[0].overhangs[0])) === fragment1[0].fSeq.substring(fSeq.length + fragment1[0].overhangs[1])) {
                            //both ends match
                            drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
                        }

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
                        }
                        else if (fragment2[0].overhangs[0] > 0
                                    &&
                                fragment2[0].cSeq.substring(0, fragment2[0].overhangs[0]).toUpperCase() === gencomSeq(genRevSeq(fragment1[0].cSeq.substring(0, fragment1[0].overhangs[0])))) {
                            //both end match
                            drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                        }

                    }

                }
            }
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
                    }
                }
                //fragment2.overhangs[0] < 0
                else {
                    if (fragment1[0].overhangs[1] > 0 && gencomSeq(fragment1[0].cSeq.substring(fragment1[0].fSeq.length - fragment1[0].overhangs[1])) === fragment2[0].fSeq.substring(0, -fragment2[0].overhangs[0]).toUpperCase()) {
                        //fragment 2 left matches with fragment1 right end
                        drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
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
                    }
                }
                //fragment2.overhangs[0] < 0
                else
                {
                    if (fragment1[0].overhangs[0] < 0 && gencomSeq(genRevSeq(fragment1[0].fSeq.substring(0, -fragment1[0].overhangs[0]))) == fragment2[0].fSeq.substring(0, -fragment2[0].overhangs[0]).toUpperCase()) {
                        //fragment2 left matches with fragment 1 left
                        drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 3); //3: direction, anticolockwise of fragment1
                    }
                }
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
                    }
                }
                //fragment2.overhangs[1] < 0
                else
                {
                    if (fragment1[0].overhangs[0] > 0 && gencomSeq(fragment1[0].cSeq.substring(0, fragment1[0].overhangs[0]).toUpperCase()) === fragment2[0].fSeq.substring(fragment2[0].fSeq.length + fragment2[0].overhangs[1]).toUpperCase()) {
                        drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 2); //2: direction, colockwise of fragment1
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
                    }
                }
                //fragment2.overhangs[1] < 0
                else {
                    if (fragment1[0].overhangs[1] < 0 && gencomSeq(genRevSeq(fragment1[0].fSeq.substring(fragment1[0].fSeq.length + fragment1[0].overhangs[1]))) === fragment2[0].fSeq.substring(fragment2[0].fSeq.length + fragment2[0].overhangs[1]).toUpperCase()) {
                        drawDirectLigation(fragment1[0], fragment2[0], "method1-cw-map", "method1-acw-map", 3); //2: direction, anticolockwise of fragment1
                    }
                }
            }
        }
    }

    //draw indriect always
    drawInDirectLigation(fragment1[0], fragment2[0], "method2-cw-map", "method2-acw-map", 1);//1: direction, both colockwise and anticolockwise

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
    console.log([idClock, idAntiClock])
    var plasmidName = $("#plasmidName").val();
    var plasmidLength = Math.max(fragment1.cSeq.length, fragment1.fSeq.length) + Math.max(fragment2.cSeq.length, fragment2.fSeq.length);
    //generate seq proper for clockwise
    var obj1 = genIndrectSeqProperty(fragment1, fragment2, true);
    //draw plasmid
    DrawPlasmid(obj1.fSeqArray, obj1.cSeqArray, true, obj1.fBluntingArray, obj1.cBluntingArray, plasmidName, plasmidLength, "#" + idClock);
    //generate seq proper for anticlockwise
    var obj2 = genIndrectSeqProperty(fragment1, fragment2, false);
    DrawPlasmid(obj2.fSeqArray, obj2.cSeqArray, false, obj2.fBluntingArray, obj2.cBluntingArray, plasmidName, plasmidLength, "#" + idAntiClock);
}

//"method1-cw-map", "method1-acw-map"
function drawDirectLigation(fragment1, fragment2, idClock, idAntiClock, direction) {
    //1: both directions
    //2: clockwise
    //3 antiClockwise
    console.log("I am trying to draw the direct ligation for you...")

}


function genIndrectSeqProperty(fragment1, fragment2, clockwise) {
    var fSeqArray = []; //fSeqFront, fSeqMiddle, fSeqEnd
    var cSeqArray = []; //cSeqFront, cSeqMiddle, cSeqEnd
    var cBluntingArray = [];
    var fBluntingArray = [];

    var f1fSeq = '';
    var c1fSeq = '';
    var f2fSeq = fragment2.fSeq;
    var c2fSeq = fragment2.cSeq;

    //f1
    if (clockwise) {
        //generate fSeq
        f1fSeq = fragment1.fSeq;
        c1fSeq = fragment1.cSeq;
        //process right of fragment1
        if (fragment1.overhangs[1] < 0) {
            var c1RightBlunting = gencomSeq(fragment1.fSeq.substring(fragment1.fSeq.length + fragment1.overhangs[1]));
            c1fSeq = c1fSeq + c1RightBlunting;
            //append cBluntingArray
            for (i = 19; i > (19 + fragment1.overhangs[1]) ; i--) {
                cBluntingArray.push(i);
            }
        }
        if (fragment1.overhangs[1] > 0) {
            var f1RightBlunting = gencomSeq(fragment1.cSeq.substring(fragment1.cSeq.length - fragment1.overhangs[1]));
            f1fSeq = f1fSeq + f1RightBlunting;
            //append fBluntingArray
            for (i = 19; i > (19 - fragment1.overhangs[1]) ; i--) {
                fBluntingArray.push(i);
            }
        }

        //left of fragment1
        if (fragment1.overhangs[0] > 0) {
            var f1LeftBlunting = gencomSeq(fragment1.cSeq.substring(0, fragment1.overhangs[0]));
            f1fSeq = f1LeftBlunting + f1fSeq;
            //append fBluntingArray
            for (i = 100; i > (100 + fragment1.overhangs[0]) ; i++) {
                fBluntingArray.push(i);
            }
        }
        if (fragment1.overhangs[0] < 0) {
            var c1LeftBlunting = gencomSeq(fragment1.fSeq.substring(0, -fragment1.overhangs[0]));
            c1fSeq = c1LeftBlunting + c1fSeq;
            //append cBluntingArray
            for (i = 100; i > (100 - fragment1.overhangs[1]) ; i++) {
                cBluntingArray.push(i);
            }
        }
    }
    else
    {
        //anticlock
        f1fSeq = genRevSeq(fragment1.cSeq);
        c1fSeq = genRevSeq(fragment1.fSeq);
        //process right of fragment1
        if (fragment1.overhangs[0] < 0) {
            var f1RightBlunting = gencomSeq(genRevSeq(fragment1.fSeq).substring(0, -fragment1.overhangs[0]));
            f1fSeq = f1fSeq + f1RightBlunting;
            //append fBluntingArray
            for (i = 19; i > (19 + fragment1.overhangs[0]) ; i--) {
                fBluntingArray.push(i);
            }
        }
        if (fragment1.overhangs[0] > 0) {
            var c1RightBlunting = gencomSeq(genRevSeq(fragment1.cSeq).substring(0, fragment1.overhangs[0]));
            c1fSeq = c1fSeq + c1RightBlunting;
            //append cBluntingArray
            for (i = 19; i > (19 - fragment1.overhangs[0]) ; i--) {
                cBluntingArray.push(i);
            }
        }


        //left of fragment1
        if (fragment1.overhangs[1] > 0) {
            var c1LeftBlunting = gencomSeq(genRevSeq(fragment1.cSeq).substring(fragment1.cSeq.length - fragment1.overhangs[1]));
            c1fSeq = c1LeftBlunting + c1fSeq;
            //append cBluntingArray
            for (i = 100; i > (100 + fragment1.overhangs[1]) ; i++) {
                cBluntingArray.push(i);
            }
        }
        if (fragment1.overhangs[1] < 0) {
            var f1LeftBlunting = gencomSeq(genRevSeq(fragment1.fSeq).substring(fragment1.fSeq.length + fragment1.overhangs[1]));
            f1fSeq = f1LeftBlunting + f1fSeq;

            //append fBluntingArray
            for (i = 100; i > (100 - fragment1.overhangs[1]) ; i++) {
                fBluntingArray.push(i);
            }
        }

    }


    ///////////////////////f2 is perfect and checked to be correct///////////////////
        //process left of fragment2
        if (fragment2.overhangs[1] > 0) {
            var f2RightBlunting = gencomSeq(fragment2.cSeq.substring(fragment2.cSeq.length - fragment2.overhangs[1]));
            f2fSeq =  f2fSeq + f2RightBlunting;
            //append fBluntingArray
            for (i = 99; i > (99 - fragment2.overhangs[1]) ; i--) {
                fBluntingArray.push(i);
            }
        }
        if (fragment2.overhangs[1] < 0) {
            var c2RightBlunting = gencomSeq(fragment2.fSeq.substring(fragment2.fSeq.length + fragment2.overhangs[1]));
            c2fSeq = c2fSeq + c2RightBlunting;
            //append cBluntingArray
            for (i = 99; i > (99 + fragment2.overhangs[1]) ; i--) {
                cBluntingArray.push(i);
            }
        }

        //right of fragment 2
        if (fragment2.overhangs[0] > 0) {
            var f2LeftBlunting = gencomSeq(fragment2.cSeq.substring(0, fragment2.overhangs[0]));
            f2fSeq =  f2LeftBlunting+ f2fSeq;
            //append fBluntingArray
            for (i = 20; i < (20 + fragment2.overhangs[0]) ; i++) {
                fBluntingArray.push(i);
            }
        }
        if (fragment2.overhangs[0] < 0) {
            var c2LeftBlunting = gencomSeq(fragment2.fSeq.substring(0, -fragment2.overhangs[0]));
            c2fSeq = c2LeftBlunting+ c2fSeq;
            //append cBluntingArray
            for (i = 20; i < (20 - fragment2.overhangs[0]) ; i++) {
                cBluntingArray.push(i);
            }
        }





        //genrate fSeqArray and cSeqArray
        var fSeqFront, fSeqMiddle, fSeqEnd;
        var cSeqFront, cSeqMiddle, cSeqEnd;

    //fragment1
    //fseq
        var f1MinLen = 40;
        if (f1fSeq.length == f1MinLen) {
            fSeqFront = f1fSeq.substring(f1fSeq.length - 20);
            fSeqEnd = f1fSeq.substring(0, 20);
        }
        else if (f1fSeq.length > f1MinLen) {
            fSeqFront = '...' + f1fSeq.substring(f1fSeq.length - 17);
            fSeqEnd = f1fSeq.substring(0, 17) + '...';
        }
        else {
            //too short
            var length = Math.floor(f1fSeq.length / 2);
            fSeqFront = genDash(f1MinLen - length) + f1fSeq.substring(f1fSeq.length - length);
            fSeqEnd = f1fSeq.substring(0, f1fSeq.length - length) + genDash(f1MinLen - length);
        }
    //cSfq
        var c1MinLen = 40;
        if (c1fSeq.length == c1MinLen) {
            cSeqFront = c1fSeq.substring(c1fSeq.length - 20);
            cSeqEnd = c1fSeq.substring(0, 20);
        }
        else if (c1fSeq.length > c1MinLen) {
            cSeqFront = '...' + c1fSeq.substring(c1fSeq.length - 17);
            cSeqEnd = c1fSeq.substring(0, 17) + '...';
        }
        else {
            //too short
            var length = Math.floor(c1fSeq.length / 2);
            cSeqFront = genDash(c1MinLen - length) + c1fSeq.substring(c1fSeq.length - length);
            cSeqEnd = c1fSeq.substring(0, c1fSeq.length - length) + genDash(c1MinLen - length);
        }


    //fragemnt2
    //fseq
        var f2MinLen = 80;
        if (f2fSeq.length == f2MinLen) {
            fSeqMiddle = f2fSeq;
        }
        else if (f2fSeq.length > f2MinLen) {
            f2fSeqFront = f2fSeq.substring(0, 37)+ '...';
            f2fSeqEnd = '...' + f2fSeq.substring(f2fSeq.length - 37);
            fSeqMiddle = f2fSeqFront + f2fSeqEnd;
        }
        else {
            //too short
            var length = Math.floor(f1fSeq.length / 2);
            f2fSeqFront = f2fSeq.substring(0, f2fSeq.length - length) + genDash(f2MinLen - length);
            f2fSeqEnd = genDash(f2MinLen - length) + f2fSeq.substring(f2fSeq.length - length);
            fSeqMiddle = f2fSeqFront + f2fSeqEnd;
        }
    //cseq
        var c2MinLen = 80;
        if (c2fSeq.length == c2MinLen) {
            cSeqMiddle = c2fSeq;
        }
        else if (c2fSeq.length > c2MinLen) {
            c2fSeqFront = c2fSeq.substring(0, 37) + '...';
            c2fSeqEnd = '...' + c2fSeq.substring(c2fSeq.length - 37);
            cSeqMiddle = c2fSeqFront + c2fSeqEnd;
        }
        else {
            //too short
            var length = Math.floor(c1fSeq.length / 2);
            c2fSeqFront = c2fSeq.substring(0, c2fSeq.length - length) + genDash(c2MinLen - length);
            c2fSeqEnd = genDash(c2MinLen - length) + c2fSeq.substring(c2fSeq.length - length);
            cSeqMiddle = c2fSeqFront + c2fSeqEnd;
        }



    //prepare for output
        fSeqArray = [fSeqFront, fSeqMiddle, fSeqEnd]; 
        cSeqArray = [cSeqFront, cSeqMiddle, cSeqEnd]; 
        cBluntingArray;
        fBluntingArray;
        var obj = {};
        obj.fSeqArray = fSeqArray;
        obj.cSeqArray = cSeqArray;
        obj.cBluntingArray = cBluntingArray;
        obj.fBluntingArray = fBluntingArray;

        return obj;
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

        debugger;
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
        obj.fragment = (i < array[0].length || i >= (120 - array[2].length)) ? "f1" : "f2";
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