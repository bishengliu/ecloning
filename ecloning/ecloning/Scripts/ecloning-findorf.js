
//this script is used to find ORF of DNA
//
var options = {
    //start nornal ATG is 0
    //alternative GTG TTG CTG is 1
    //all is 2
    startCodon : 0,


    //stop codons are:TAA TAG  TGA
            //TAA is 1
            //TAG is 2
            //TGA is 3
            //all is 0
    stopCodon : 0,


    //frame 1
    //frame 2
    //feame 3
    //frame 1, 2, 3 is 0
    frame : 0,


    // 0 is both directions
    //1 is forward
    //2 is reverse
    direction : 0,

    //minSzie = 30
    minSzie : 30,


    sequence : ""
};

function findROF(options) {
    var direction = options.direction;
    var minSize = options.minSzie;
    var sequence = options.sequence;
    var frame = options.frame;
    var stopCodon = options.stopCodon;
    var startCodon = options.startCodon;

    var orfObj = [];

    if (direction == 0) {

    }
    else if (direction == 1) {

    }
    else {
        //direction ==2
    }


    return orfObj;
}