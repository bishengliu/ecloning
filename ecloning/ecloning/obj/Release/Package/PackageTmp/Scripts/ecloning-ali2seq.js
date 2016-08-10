function ali2seq(seq1, seq2, seq1Name, seq2Name, aliConatinerId, colorscheme) {

    //align the seq
    var seqsAligned = stringalign(seq1, seq2, 1, 1, .5);

    //prepare for msa viewer
    var seqs = [{ name: seq1Name, id: 0, seq: seqsAligned.string1 }, { name: seq2Name, id: 1, seq: seqsAligned.string2 }];
    var aliConatiner = $('#' + aliConatinerId);
    var opts = {
        el: aliConatiner,
        seqs: seqs,
        vis: {
            conserv: false,
            overviewbox: false,
            seqlogo: false
        },
        conf: {
            dropImport: true
        },
        zoomer: {
            menuFontsize: "12px",
            autoResize: true,
            alignmentHeight: 50
        },
        colorscheme: { "scheme": colorscheme }
    };
    var m = new msa.msa(opts);
    m.render()

}