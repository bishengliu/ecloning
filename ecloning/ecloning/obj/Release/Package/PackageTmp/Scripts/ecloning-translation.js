//this is the funciton of DNA==> RNA transcrition and RNA=>protein translation

function transcription(seq) {
    var array = seq.toUpperCase().split('');
    var rnaArray = [];
    $.each(array, function (i, d) {
        if (d == "T") {
            rnaArray.push("U");
        }
        else {
            rnaArray.push(d);
        }
    })
    return rnaArray.join('');
}


function translation(seq, seqType, type) {
    //if type is true then return one letter AA codon
    //if false return 3-ltter AA codon
    var newSeq = [];
    if (seqType == "DNA") {
        newSeq = seq.toUpperCase();
    }
    else {
        //rna
        newSeq = ConvertToDNA(seq).split('').join('');
    }
    
    var proArray = [];
    for (i = 0; i < newSeq.length; i += 3) {
        var AA = convertoRNA(newSeq.substring(i, i + 3), type);
        if (AA == "STOP") {
            break;
        }
        else {
            proArray.push(AA);
        }
    }
    return proArray.join('');
}

function ConvertToDNA(seq) {
    var array = seq.toUpperCase().split('');
    var dnaArray = [];
    $.each(array, function (i, d) {
        if (d == "U") {
            dnaArray.push("T");
        }
        else {
            dnaArray.push(d);
        }
    })
    return dnaArray.join('');
}

function convertoRNA(code, type) {
    var AA = "";
    if (type == true) {
        //one letter
        if (code === "TTT" || code === "TTC") {
            AA = "F";
        }
        if (code === "TTA" || code === "TTG" || code === "CTT" || code === "CTC" || code === "CTA" || code === "CTG") {
            AA = "L";
        }
        if (code === "ATT" || code === "ATC" || code === "ATA") {
            AA = "I";
        }
        if (code === "ATG") {
            AA = "M";
        }
        if (code === "GTT" || code === "GTC" || code === "GTA" || code === "GTG") {
            AA = "V";
        }

        if (code === "TCT" || code === "TCC" || code === "TCA" || code === "TCG") {
            AA = "S";
        }
        if (code === "CCT" || code === "CCC" || code === "CCA" || code === "CCG") {
            AA = "P";
        }
        if (code === "ACT" || code === "ACC" || code === "ACA" || code === "ACG") {
            AA = "T";
        }
        if (code === "GCT" || code === "GCC" || code === "GCA" || code === "GCG") {
            AA = "A";
        }

        if (code === "TAT" || code === "TAC" ) {
            AA = "Y";
        }
        if (code === "TAA" || code === "TAG") {
            AA = "STOP";
        }
        if (code === "CAT" || code === "CAC") {
            AA = "H";
        }
        if (code === "CAA" || code === "CAG") {
            AA = "Q";
        }
        if (code === "AAT" || code === "AAC") {
            AA = "N";
        }
        if (code === "AAA" || code === "AAG") {
            AA = "K";
        }
        if (code === "GAT" || code === "GAC") {
            AA = "D";
        }
        if (code === "GAA" || code === "GAG") {
            AA = "E";
        }
        if (code === "TGT" || code === "TGC") {
            AA = "C";
        }
        if (code === "TGA") {
            AA = "STOP";
        }
        if (code === "TGG") {
            AA = "W";
        }
        if (code === "CGT" || code === "CGC" || code === "CGA" || code === "CGG") {
            AA = "R";
        }
        if (code === "AGT" || code === "AGC") {
            AA = "S";
        }
        if (code === "AGA" || code === "AGG") {
            AA = "R";
        }
        if (code === "GGT" || code === "GGC" || code === "GGA" || code === "GGG") {
            AA = "G";
        }
    }
    else {
        //3 letter
        if (code === "TTT" || code === "TTC") {
            AA = "Phe";
        }
        if (code === "TTA" || code === "TTG" || code === "CTT" || code === "CTC" || code === "CTA" || code === "CTG") {
            AA = "Leu";
        }
        if (code === "ATT" || code === "ATC" || code === "ATA") {
            AA = "Ile";
        }
        if (code === "ATG") {
            AA = "Met";
        }
        if (code === "GTT" || code === "GTC" || code === "GTA" || code === "GTG") {
            AA = "Val";
        }

        if (code === "TCT" || code === "TCC" || code === "TCA" || code === "TCG") {
            AA = "Ser";
        }
        if (code === "CCT" || code === "CCC" || code === "CCA" || code === "CCG") {
            AA = "Pro";
        }
        if (code === "ACT" || code === "ACC" || code === "ACA" || code === "ACG") {
            AA = "Thr";
        }
        if (code === "GCT" || code === "GCC" || code === "GCA" || code === "GCG") {
            AA = "Ala";
        }

        if (code === "TAT" || code === "TAC") {
            AA = "Tyr";
        }
        if (code === "TAA" || code === "TAG") {
            AA = "STOP";
        }
        if (code === "CAT" || code === "CAC") {
            AA = "His";
        }
        if (code === "CAA" || code === "CAG") {
            AA = "Gln";
        }
        if (code === "AAT" || code === "AAC") {
            AA = "Asn";
        }
        if (code === "AAA" || code === "AAG") {
            AA = "Lys";
        }
        if (code === "GAT" || code === "GAC") {
            AA = "Asp";
        }
        if (code === "GAA" || code === "GAG") {
            AA = "Glu";
        }
        if (code === "TGT" || code === "TGC") {
            AA = "Cys";
        }
        if (code === "TGA") {
            AA = "STOP";
        }
        if (code === "TGG") {
            AA = "Trp";
        }
        if (code === "CGT" || code === "CGC" || code === "CGA" || code === "CGG") {
            AA = "Arg";
        }
        if (code === "AGT" || code === "AGC") {
            AA = "Ser";
        }
        if (code === "AGA" || code === "AGG") {
            AA = "Arg";
        }
        if (code === "GGT" || code === "GGC" || code === "GGA" || code === "GGG") {
            AA = "Gly";
        }
    }

    return AA;
}