const path = require('path');
const webpack = require('webpack');

module.exports = {
    entry: './src/index.js',
    output: {
        path: path.resolve('Content/dist'),
        filename: 'bundle.js',
        sourceMapFilename: "bundle.map"
    },
    //resolve: {
    //    extensions: ['', '.js', '.jsx']
    //},
    module: {
        loaders: [
            {
                test: /\.jsx?$/,
                exclude: /(node_modules)/,
                loader: 'babel-loader',
                query: {
                    presets: ['es2015', 'env', 'stage-0', 'react']
                }
            }
        ]
    },
    plugins: [
        new webpack.optimize.UglifyJsPlugin({
            sourceMap: false,
            warnings: false,
            mangle: true
        })
    ]
}