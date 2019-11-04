const path = require("path");
const HtmlWebPackPlugin = require("html-webpack-plugin");

const HWPIndex = new HtmlWebPackPlugin({
  template: "./src/index.html",
  filename: "./index.html"
});

const HWPSlave = new HtmlWebPackPlugin({
  template: "./src/slave.html",
  filename: './slave.html'
});

const config = {
  target: "electron-renderer",
  devtool: "source-map",
  entry: "./src/renderer/renderer.tsx",
  output: {
    filename: "renderer.js",
    path: path.resolve(__dirname, "dist")
  },
  module: {
    rules: [
      {
        test: /\.(ts|tsx)$/,
        exclude: /node_modules/,
        use: {
          loader: "babel-loader"
        }
      },
      {
        test: /\.(scss)$/,
        use: [{
          loader: 'style-loader', // inject CSS to page
        }, {
          loader: 'css-loader', // translates CSS into CommonJS modules
        }, {
          loader: 'postcss-loader', // Run post css actions
          options: {
            plugins: function () { // post css plugins, can be exported to postcss.config.js
              return [
                require('precss'),
                require('autoprefixer')
              ];
            }
          }
        }, {
          loader: 'sass-loader' // compiles Sass to CSS
        }]
      },
      {
        test: /\.(png|svg|jpg|gif)$/,
        use: [
          'file-loader'
        ]
      }
    ]
  },
  resolve: {
    extensions: [".ts", ".tsx", ".js"]
  },
  plugins: [HWPIndex, HWPSlave]
};

module.exports = (env, argv) => {
  return config;
};