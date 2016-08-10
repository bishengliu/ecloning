
//this script is used to find ORF of DNA
//
var options = {
    //start nornal ATG is 0
    //alternative GTG TTG CTG is 1
    //all is 2
    'startCodon' : 0,


    //stop codons are:TAA TAG  TGA
            //TAA is 1
            //TAG is 2
            //TGA is 3
            //all is 0
    'stopCodon' : 0,


    //frame 1
    //frame 2
    //feame 3
    //frame 1, 2, 3 is 0
    'frame' : 0,


    //minSzie = 30
    'minSize' : 30,


    'sequence' : ""
};

function findROF(options) {
    var minSize = options.minSize;
    var sequence = options.sequence;
    var frame = options.frame;
    var stopCodon = options.stopCodon;
    var startCodon = options.startCodon;

    //orfobj = {
    //  frame: frame,
    //  start: startIndex,
    //  stop: stopIndex
    //}
    var orfObj = [];
    var fORF = [];
    var rORF = [];

    //forward only
    if (frame != 0) {
        //frame 1, 2 or 3
        fORF = ForwardORF(frame, sequence, startCodon, stopCodon, minSize);
    }
    else {
        //frame 1, 2 and 3
        var fORF1 = ForwardORF(1, sequence, startCodon, stopCodon, minSize);
        var fORF2 = ForwardORF(2, sequence, startCodon, stopCodon, minSize);
        var fORF3 = ForwardORF(3, sequence, startCodon, stopCodon, minSize);
        $.merge(fORF1, fORF2, fORF3);
    }


    orfObj = $.merge(fORF, rORF);
    return orfObj;
}

//orfobj = {
//  frame: frame,
//  start: startIndex,
//  stop: stopIndex
//}

function ForwardORF(frame, sequence, startCodon, stopCodon, minSize)
{
    sequence = sequence.toUpperCase();
    //frame can only be 1, 2 and 3
    //prepare tempSeq according to reading frame
    var strFront, tempSeq;
    if (frame == 1) {
        strFront = "";
        tempSeq = sequence;
    }
    else {
        strFront = sequence.substring(0, (frame - 1));
        tempSeq = sequence.substring(frame-1) + strFront;
    }

    startPos1 = [];
    startPos2 = [];
    startPos3 = [];
    //check start codon
    if (frame == 1) {
        if (startCodon == 0) {
            //ATG
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) === "ATG") {
                    var index = i + (frame - 1);
                    startPos1.push(index);
                }
            }
        }
        if (startCodon == 1) {
            // "GTG", "TTG", "CTG"
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "GTG" || tempSeq.substring(i, i + 3) == "TTG" || tempSeq.substring(i, i + 3) == "CTG") {
                    var index = i + (frame - 1);
                    startPos1.push(index);
                }
            }
        }
        if (startCodon == 2) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "ATG" || tempSeq.substring(i, i + 3) == "GTG" || tempSeq.substring(i, i + 3) == "TTG" || tempSeq.substring(i, i + 3) == "CTG") {
                    var index = i + (frame - 1);
                    startPos1.push(index);
                }
            }
        }
    }
    else if (frame == 2) {
        if (startCodon == 0) {
            //ATG
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) === "ATG") {
                    var index = i + (frame - 1);
                    startPos2.push(index);
                }
            }
        }
        if (startCodon == 1) {
            // "GTG", "TTG", "CTG"
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "GTG" || tempSeq.substring(i, i + 3) == "TTG" || tempSeq.substring(i, i + 3) == "CTG") {
                    var index = i + (frame - 1);
                    startPos2.push(index);
                }
            }
        }
        if (startCodon == 2) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "ATG" || tempSeq.substring(i, i + 3) == "GTG" || tempSeq.substring(i, i + 3) == "TTG" || tempSeq.substring(i, i + 3) == "CTG") {
                    var index = i + (frame - 1);
                    startPos2.push(index);
                }
            }
        }
    }
    else {
        //frame 3
        if (startCodon == 0) {
            //ATG
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) === "ATG") {
                    var index = i + (frame - 1);
                    startPos3.push(index);
                }
            }
        }
        if (startCodon == 1) {
            // "GTG", "TTG", "CTG"
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "GTG" || tempSeq.substring(i, i + 3) == "TTG" || tempSeq.substring(i, i + 3) == "CTG") {
                    var index = i + (frame - 1);
                    startPos3.push(index);
                }
            }
        }
        if (startCodon == 2) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "ATG" || tempSeq.substring(i, i + 3) == "GTG" || tempSeq.substring(i, i + 3) == "TTG" || tempSeq.substring(i, i + 3) == "CTG") {
                    var index = i + (frame - 1);
                    startPos3.push(index);
                }
            }
        }
    }


    stopPos1 = [];
    stopPos2 = [];
    stopPos3 = [];
    //stop codes
    if (frame == 1) {
        if (stopCodon == 0) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "TAA" || tempSeq.substring(i, i + 3) == "TAG" || tempSeq.substring(i, i + 3) == "TGA") {
                    var index = i + (frame - 1);
                    stopPos1.push(index);
                }
            }
        }
        if (stopCodon == 1) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "TAA") {
                    var index = i + (frame - 1);
                    stopPos1.push(index);
                }
            }
        }
        if (stopCodon == 2) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "TAG") {
                    var index = i + (frame - 1);
                    stopPos1.push(index);
                }
            }
        }
        if (stopCodon == 3) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "TGA") {
                    var index = i + (frame - 1);
                    stopPos1.push(index);
                }
            }
        }
    } else if (frame == 2) {
        if (stopCodon == 0) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "TAA" || tempSeq.substring(i, i + 3) == "TAG" || tempSeq.substring(i, i + 3) == "TGA") {
                    var index = i + (frame - 1);
                    stopPos2.push(index);
                }
            }
        }
        if (stopCodon == 1) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "TAA") {
                    var index = i + (frame - 1);
                    stopPos2.push(index);
                }
            }
        }
        if (stopCodon == 2) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "TAG") {
                    var index = i + (frame - 1);
                    stopPos2.push(index);
                }
            }
        }
        if (stopCodon == 3) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "TGA") {
                    var index = i + (frame - 1);
                    stopPos2.push(index);
                }
            }
        }
    }
    else {
        //frame ==3
        if (stopCodon == 0) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "TAA" || tempSeq.substring(i, i + 3) == "TAG" || tempSeq.substring(i, i + 3) == "TGA") {
                    var index = i + (frame - 1);
                    stopPos3.push(index);
                }
            }
        }
        if (stopCodon == 1) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "TAA") {
                    var index = i + (frame - 1);
                    stopPos3.push(index);
                }
            }
        }
        if (stopCodon == 2) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "TAG") {
                    var index = i + (frame - 1);
                    stopPos3.push(index);
                }
            }
        }
        if (stopCodon == 3) {
            //all
            for (var i = 0; i < (tempSeq.length - 2) ; i += 3) {
                if (tempSeq.substring(i, i + 3) == "TGA") {
                    var index = i + (frame - 1);
                    stopPos3.push(index);
                }
            }
        }
    }

    startPos1.sort();
    startPos2.sort();
    startPos3.sort();
    stopPos1.sort();
    stopPos2.sort();
    stopPos3.sort();

    var f1Array = f2Array = f3Array = [];
    //zip startPos and endPos
    //f1Array
    if (startPos1.length > 0 && stopPos1.length > 0) {
        var tempStart = -1;
        $.each(stopPos1, function (i, sp) {
            $.each(startPos1, function (i, st) {
                if (st < tempStart) {
                    return;
                }
                if (st <= (sp - minSize)) {
                    var obj = {
                        'frame': 1,
                        'start': st,
                        'stop': sp
                    }
                    f1Array.push(obj);
                    tempStart = sp;
                    return false;
                }
                else {
                    tempStart = sp > st ? sp : st;
                    return false;
                }
            })
        })
    }
   
    //f2Array
    if (startPos2.length > 0 && stopPos2.length > 0) {
        var tempStart = -1;
        $.each(stopPos2, function (i, sp) {
            $.each(startPos2, function (i, st) {
                if (st < tempStart) {
                    return;
                }
                if (st <= (sp - minSize)) {
                    var obj = {
                        'frame': 2,
                        'start': st,
                        'stop': sp
                    }
                    f2Array.push(obj);
                    tempStart = sp;
                    return false;
                }
                else {
                    tempStart = sp > st ? sp : st;
                    return false;
                }
            })
        })
    }

    //f3Array
    if (startPos3.length > 0 && stopPos3.length > 0) {
        var tempStart = -1;
        $.each(stopPos3, function (i, sp) {
            $.each(startPos3, function (i, st) {
                if (st < tempStart) {
                    return;
                }
                if (st <= (sp - minSize)) {
                    var obj = {
                        'frame': 3,
                        'start': st,
                        'stop': sp
                    }
                    f3Array.push(obj);
                    tempStart = sp;
                    return false;
                }
                else {
                    tempStart = sp > st ? sp : st;
                    return false;
                }
            })
        })
    }

    if (frame == 1) {
        return f1Array;
    }
    else if (frame == 2) {
        return f2Array;
    }
    else {
        return f3Array;
    }
}



// Returns array of sequence, split from input sequence
function wrap(seq_string, width) {
    if (seq_string) {
        var res = [];
        var line = '';
        for (var i = 0; i < seq_string.length; i++) {
            line += seq_string.charAt(i);
            if (line.length >= width) {
                res.push(line);
                line = '';
            }
        }
        if (line.length > 0) { res.push(line); }
        return res;
    }
    return [];
}