var gd = new GiraffeDraw();

function start_giraffe_analyze(name, linear_map) {
    GiraffeAnalyze(jQuery, gd,{
        'dom_id':'giraffe-analyze','name':name,'analyzer_width':1340,
        'linear_map':linear_map
    });

    seq_view = jQuery(".giraffe-viewer");
    map_view = jQuery(".giraffe-tabs");
    //search_view = jQuery("#giraffe-viewer-search-container");
    //jQuery("#results_div").append(search_view);
    jQuery("#results_div").append(seq_view);
    jQuery("#results_div").append(map_view);

    //jQuery(".giraffe-viewer-topbar").remove(search_view);
    jQuery("#giraffe-analyze").remove();
}

function init_giraffe_analyze(sequence_id, sequence_time, name, linear_map) {
    var analysis_pending_html =
        '<p>This sequence is still being analyzed. Please wait...</p>';
    var analysis_done_html = (
        '<p>The sequence analysis is complete.' +
        ' <a href="" onclick="window.location.reload();">Refresh</a> to see' +
        ' the results.</p>'
    );

    var check_interval;
    var map_tab;
    var digest_tab;

    function indicate_pending() {
        // Hack up interface to show a 'analysis running' message
        // instead of things that require the feature analysis results.
        // TODO Actually integrate this in to the Giraffe UI JS
        map_tab.html(analysis_pending_html);
        digest_tab.html(analysis_pending_html);
        // Every 5 seconds should be sufficient, since any sequence that takes
        // long enough to analyze that the user will even run this code will
        // probably take a while.
        check_interval = window.setInterval(check_completion, 5000);
    }

    function check_completion() {
        jQuery.get(
            "/tools/analyze/" + sequence_id + "/analyzed",
            function(analyzed) {
                if (! analyzed) {
                    return;
                }
                // simply calling gd.read() again doesn't seem to work, so just
                // prompt for a page-refresh.
                map_tab.html(analysis_done_html);
                digest_tab.html(analysis_done_html);
                // don't keep running this function
                window.clearInterval(check_interval);
            }
        );
    }

    jQuery.get(
        "/giraffe/blat/" + sequence_id + "/default",
        {
            sequence: 1,
            mtime: sequence_time
        },
        function(data) {
            gd.read(data);
            start_giraffe_analyze(name, linear_map);
            // We have to set these here since they weren't created until we
            // called start_giraffe_analyze(), and the child elements we select
            // on will disapper if indicate_pending() is run.
            map_tab = jQuery('div.giraffe-tab').has('#giraffe-map-help');
            digest_tab = jQuery('div.giraffe-tab').has('#giraffe-digest-data');
            if (data[3] == "pending") {
                indicate_pending();
            }
        }
    );
}
