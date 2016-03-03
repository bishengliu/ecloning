//this is the client side validation for maximum upload file size. 
//the maximum allowed upload file size is configured in web.confg, currently it is set to be 10MB


//all these functions work only with HTML5
function uploadSize(uploadId, outputId) {
    if (typeof ($('#'+ uploadId)[0].files) != "undefined") {
        //check upload file size
        alert("1");
        var size = parseFloat($('#' + uploadId)[0].files[0].size / 1024 / 1024).toFixed(2); // cal the file size in MB
        if (size > 10) {
            $('#' + outputId).text('Maximum upload file size is 10MB!');
            return false;
        }
    }
}

function uploadImage(uploadId, outputId)
{
    if (typeof ($('#' + uploadId)[0].files) != "undefined")
    {
        //check the upload file type
        var type = $('#' + uploadId)[0].files[0].type;
        if (type == "image/png" || type == "image/jpeg" || type == "image/tiff" || type == "image/bmp") {
            return true;
        }
        else {
            $('#' + outputId).text('Only allow for ".bmp", ".jpg", ".tiff" and ".png"!');
            return false;
        }
    }

}