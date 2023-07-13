const {env} = require('process');

//const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
//  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:31077';
/*
const PROXY_CONFIG = [
  {
    context: [
      "/api",
    ],
    proxyTimeout: 3000,
    target: "http://localhost:5242",
    secure: false,
    headers: {
      Connection: 'Keep-Alive'
    }
  }
]
*/

const PROXY_CONFIG =
  {
    "/api/*": {
      "target": "http://localhost:5242",
      "secure": false,
      "changeOrigin": false,
      "logLevel": "debug"
    }
  }
;

module.exports = PROXY_CONFIG;
