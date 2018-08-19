var express = require('express');
var fs = require('fs');
var request = require('request');
var cheerio = require('cheerio');
var app = express();


app.get('/scrape_/', function (req, res) {
    var url = "";
    console.log('Scraping from %s', url);

});


app.get('/scrape_momjunction/:pageNo/', function (req, res) {
    var url = "https://www.momjunction.com/baby-names/sikh/page/" + req.params.pageNo + "/";
    console.log('Scraping from %s',url);
    var count = 0;
    var json = { names: [], meanings: [], genders: [], origins: [], religions: [] };

    request(url, function (error, response, html) {
        if (!error) {
            var $ = cheerio.load(html);

            var names = $('.table-responsive1 tbody tr td a[class *= "name"]');
            var genderList = $('.table-responsive1 tbody tr td[class *= "gender"]');
            var meaningList = $('.table-responsive1 tbody tr td[class *= "meaning"]');
            var originList = $('.table-responsive1 tbody tr td[class *= "origin"]');
            var religionList = $('.table-responsive1 tbody tr td[class *= "religion"]');

            names.each(function (index) {
                json.names[index] = $(this).text().trim();
            });

            genderList.each(function (index) {
                json.genders[index] = $(this).text().trim();
            });

            meaningList.each(function (index) {
                json.meanings[index] = $(this).text().trim();
            });

            originList.each(function (index) {
                json.origins[index] = $(this).text().trim();
            });

            religionList.each(function (index) {
                json.religions[index] = $(this).text().trim();
            });

            console.log('Check JSON Object');
            console.log('Total Names: ' + json.names.length);
            console.log('Total Genders: ' + json.genders.length);
            console.log('Total Meanings: ' + json.meanings.length);
            console.log('Total Origins: ' + json.origins.length);
            console.log('Total Religions: ' + json.religions.length);
        }
        var filename = "output" + req.params.pageNo + ".json";
        fs.writeFile(filename, JSON.stringify(json, null, 4), function (err) {
            console.log('File successfully written! - Check your project directory for the ' + filename + ' file. ' + err);
        })

        res.send('Check your console!')
    });
});

app.listen('8081');
console.log('Magic happens on port 8081');
exports = module.exports = app;