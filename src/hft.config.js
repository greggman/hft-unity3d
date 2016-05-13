const webpack = require('webpack');
const path = require('path');

module.exports = {
    entry: './hft.js',
    output: {
        path: path.join(__dirname, '../Assets/WebPlayerTemplates/HappyFunTimes/hft'),
        filename: 'hft.js',
    },
}

