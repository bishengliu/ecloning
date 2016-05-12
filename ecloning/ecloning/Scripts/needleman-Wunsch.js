//this is the js for aligning 2 strings using the Needleman–Wunsch algorithm
//info on Needleman–Wunsch algorithm is here: https://en.wikipedia.org/wiki/Needleman–Wunsch_algorithm

//mispen == mismatch penalty == 1
//gappen == gap penalty == 1
//match  =  -1
//skwpen == skey penalty = 0.5
function stringalign(astr, bstr, mispen, gappen, skwpen) {
    ain = astr.split(''); //change to array
    bin = bstr.split('');
    var i, j, k;
    var dn, rt, dg;
    var ia = ain.length, ib = bin.length; //get the array length
    var aout = []; // .resize(ia+ib);
    var bout = [];
    var sp = '-';
    var summary = []; //for the summary, not neccessary

    //generate the grid for scoring
    var cost = []; 
    var marked = []; 
    for (n = 0 ; n < ia + 1 ; ++n) {
        cost[n] = new Array(ib + 1);
        marked[n] = new Array(ib + 1);
    }
    //fill the grid
    cost[0][0] = 0.; //set the col1 and row1 to be 0;
    for (i = 1; i <= ia; i++) cost[i][0] = cost[i - 1][0] + skwpen; //col1
    for (i = 1; i <= ib; i++) cost[0][i] = cost[0][i - 1] + skwpen; //row1
    for (i = 1; i <= ia; i++) for (j = 1; j <= ib; j++) {
        dn = cost[i - 1][j] + ((j == ib) ? skwpen : gappen); //top
        rt = cost[i][j - 1] + ((i == ia) ? skwpen : gappen); //left
        dg = cost[i - 1][j - 1] + ((ain[i - 1] == bin[j - 1]) ? -1. : mispen); //diag, if match =-1
        cost[i][j] = Math.min(dn, rt, dg); //fill the min, looking for best match
    }

    //get the result based on the scoring
    i = ia; j = ib; k = 0;
    while (i > 0 || j > 0) {
        marked[i][j] = 1;
        dn = rt = dg = 9.99e99;
        if (i > 0) dn = cost[i - 1][j] + ((j == ib) ? skwpen : gappen);
        if (j > 0) rt = cost[i][j - 1] + ((i == ia) ? skwpen : gappen);
        if (i > 0 && j > 0) dg = cost[i - 1][j - 1] +
                           ((ain[i - 1] == bin[j - 1]) ? -1. : mispen);
        if (dg <= Math.min(dn, rt)) {
            aout[k] = ain[i - 1];
            bout[k] = bin[j - 1];
            summary[k++] = ((ain[i - 1] == bin[j - 1]) ? '=' : '!');
            i--; j--;
        }
        else if (dn < rt) {
            aout[k] = ain[i - 1];
            bout[k] = sp;
            summary[k++] = sp;
            i--;
        }
        else {
            aout[k] = sp;
            bout[k] = bin[j - 1];
            summary[k++] = sp;
            j--;
        }
        marked[i][j] = 1;
    }
    for (i = 0; i < k / 2; i++) {
        var t = aout[k - 1 - i];
        aout[k - 1 - i] = aout[i];
        aout[i] = t;

        t = bout[k - 1 - i];
        bout[k - 1 - i] = bout[i];
        bout[i] = t;

        t = summary[k - 1 - i];
        summary[k - 1 - i] = summary[i];
        summary[i] = t;
    }

    //output array
    aout.size = k; bout.size = k; summary.size = k;

    var outObject;
    outObject = { string1: aout.join(''), string2: bout.join('') };
    return outObject;
    //    console.log(aout.join(''));
    //   console.log(bout.join(''));
    //    console.log(summary.join(''));
    /*
    //==========table out put the grid for the best score
    var table = "";
    table += "<tr><td></td>";
    for (n = 0; n < bin.length; ++n)
        table += "<th>" + bin[n] + "</th>";
    table += "</tr>";
    for (n = 0; n < cost.length; ++n) {
        table += "<tr>";
        if (n < cost.length - 1)
            table += "<th>" + ain[n] + "</th>";
        else
            table += "<td></td>";
        for (m = 0; m < cost[n].length; ++m) {
            if (marked[n][m] == 1)
                table += '<td align="center" bgcolor="#ff0000">' + cost[n][m] + "</td>";
            else
                table += '<td align="center">' + cost[n][m] + "</td>";
        }
        table += "</tr>";        
    }
    $("#mat").html(table);
    $("#newA").html(aout.join("").replace(/ /g, "&nbsp;"));
    $("#newB").html(bout.join("").replace(/ /g, "&nbsp;"));
    $("#summary").html(summary.join("").replace(/ /g, "&nbsp;"));
    //========== end table out put===============
    */
}