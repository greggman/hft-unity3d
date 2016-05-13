const webpack = require('webpack');
const path = require('path');

module.exports = {
    entry: './audiomanager.js',
    output: {
        path: path.join(__dirname, '../Assets/WebPlayerTemplates/HappyFunTimes/3rdparty'),
        filename: 'audiomanager.js',
    },
};


