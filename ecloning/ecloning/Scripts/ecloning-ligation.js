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
    //decide which method the ligation can use
    if (fragment1.overhangs[0] == 0 && fragment1.overhangs[1] == 0 && fragment2.overhangs[0] == 0 && fragment2.overhangs[1] == 0) {
        //both end are blunt ends
        //ligation directly, 
        //can be clockwise and anticlockwise
    }
    if (fragment1.overhangs[0] != 0 && fragment1.overhangs[1] != 0 && fragment2.overhangs[0] != 0 && fragment2.overhangs[1] != 0) {
        //both are stick ends
        //can have 2 methods
        //if matched, then directly ligated
        //not, then first extension the ligation
    }    
    if (((fragment1.overhangs[0] == 0 && fragment1.overhangs[1] != 0) || (fragment1.overhangs[0] != 0 && fragment1.overhangs[1] == 0)) && ((fragment2.overhangs[0] == 0 && fragment2.overhangs[1] != 0) || (fragment2.overhangs[0] != 0 && fragment2.overhangs[1] == 0))) {
        //fragment1 and 2: one stick-end and one blunt-end
        //if matched, then directly ligated
        //not, then first extension the ligation
    }
    if (((fragment1.overhangs[0] == 0 && fragment1.overhangs[1] != 0) || (fragment1.overhangs[0] != 0 && fragment1.overhangs[1] == 0)) && (fragment2.overhangs[0] != 0 && fragment2.overhangs[1] != 0)) {
        //fragment1: one stick-end and one blunt-end
        //fragment2: 2 stick-ends
        //can only be extened first and then ligate
        //can be clockwise and anticlockwise
    }
    if (((fragment2.overhangs[0] == 0 && fragment2.overhangs[1] != 0) || (fragment2.overhangs[0] != 0 && fragment2.overhangs[1] == 0)) && (fragment1.overhangs[0] != 0 && fragment1.overhangs[1] != 0)) {
        //fragment2: one stick-end and one blunt-end
        //fragment1: 2 stick-ends
        //can only be extened first and then ligate
        //can be clockwise and anticlockwise
    }

}