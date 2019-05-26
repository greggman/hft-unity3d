const webpack = require('webpack');
const path = require('path');
var plugins = require('webpack-load-plugins')();

module.exports = {
    entry: './game-utils.js',
    output: {
        path: path.join(__dirname, '../Assets/WebPlayerTemplates/HappyFunTimes/3rdparty'),
        filename: 'game-utils.js',
    },
};


