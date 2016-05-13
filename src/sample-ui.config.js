const webpack = require('webpack');
const path = require('path');

module.exports = {
    entry: './sample-ui.js',
    output: {
        path: path.join(__dirname, '../Assets/WebPlayerTemplates/HappyFunTimes/sample-ui'),
        filename: 'sample-ui.js',
    },
}

