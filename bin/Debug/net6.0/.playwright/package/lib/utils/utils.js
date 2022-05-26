"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.SigIntWatcher = void 0;
exports.arrayToObject = arrayToObject;
exports.assert = assert;
exports.calculateSha1 = calculateSha1;
exports.canAccessFile = canAccessFile;
exports.constructURLBasedOnBaseURL = constructURLBasedOnBaseURL;
exports.createGuid = createGuid;
exports.debugAssert = debugAssert;
exports.debugMode = debugMode;
exports.download = download;
exports.existsAsync = void 0;
exports.fetchData = fetchData;
exports.getAsBooleanFromENV = getAsBooleanFromENV;
exports.getFromENV = getFromENV;
exports.getPlaywrightVersion = getPlaywrightVersion;
exports.getUserAgent = getUserAgent;
exports.headersArrayToObject = headersArrayToObject;
exports.headersObjectToArray = headersObjectToArray;
exports.hostPlatform = void 0;
exports.isError = isError;
exports.isFilePayload = isFilePayload;
exports.isObject = isObject;
exports.isRegExp = isRegExp;
exports.isString = isString;
exports.isUnderTest = isUnderTest;
exports.makeWaitForNextTask = makeWaitForNextTask;
exports.mkdirIfNeeded = mkdirIfNeeded;
exports.monotonicTime = monotonicTime;
exports.objectToArray = objectToArray;
exports.removeFolders = removeFolders;
exports.setUnderTest = setUnderTest;
exports.spawnAsync = spawnAsync;
exports.streamToString = streamToString;
exports.transformCommandsForRoot = transformCommandsForRoot;
exports.wrapInASCIIBox = wrapInASCIIBox;

var _path = _interopRequireDefault(require("path"));

var _fs = _interopRequireDefault(require("fs"));

var _rimraf = _interopRequireDefault(require("rimraf"));

var crypto = _interopRequireWildcard(require("crypto"));

var _os = _interopRequireDefault(require("os"));

var _http = _interopRequireDefault(require("http"));

var _https = _interopRequireDefault(require("https"));

var _child_process = require("child_process");

var _proxyFromEnv = require("proxy-from-env");

var URL = _interopRequireWildcard(require("url"));

var _ubuntuVersion = require("./ubuntuVersion");

var _progress = _interopRequireDefault(require("progress"));

function _getRequireWildcardCache(nodeInterop) { if (typeof WeakMap !== "function") return null; var cacheBabelInterop = new WeakMap(); var cacheNodeInterop = new WeakMap(); return (_getRequireWildcardCache = function (nodeInterop) { return nodeInterop ? cacheNodeInterop : cacheBabelInterop; })(nodeInterop); }

function _interopRequireWildcard(obj, nodeInterop) { if (!nodeInterop && obj && obj.__esModule) { return obj; } if (obj === null || typeof obj !== "object" && typeof obj !== "function") { return { default: obj }; } var cache = _getRequireWildcardCache(nodeInterop); if (cache && cache.has(obj)) { return cache.get(obj); } var newObj = {}; var hasPropertyDescriptor = Object.defineProperty && Object.getOwnPropertyDescriptor; for (var key in obj) { if (key !== "default" && Object.prototype.hasOwnProperty.call(obj, key)) { var desc = hasPropertyDescriptor ? Object.getOwnPropertyDescriptor(obj, key) : null; if (desc && (desc.get || desc.set)) { Object.defineProperty(newObj, key, desc); } else { newObj[key] = obj[key]; } } } newObj.default = obj; if (cache) { cache.set(obj, newObj); } return newObj; }

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

/**
 * Copyright (c) Microsoft Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
// `https-proxy-agent` v5 is written in TypeScript and exposes generated types.
// However, as of June 2020, its types are generated with tsconfig that enables
// `esModuleInterop` option.
//
// As a result, we can't depend on the package unless we enable the option
// for our codebase. Instead of doing this, we abuse "require" to import module
// without types.
const ProxyAgent = require('https-proxy-agent');

const existsAsync = path => new Promise(resolve => _fs.default.stat(path, err => resolve(!err)));

exports.existsAsync = existsAsync;

function httpRequest(params, onResponse, onError) {
  const parsedUrl = URL.parse(params.url);
  let options = { ...parsedUrl
  };
  options.method = params.method || 'GET';
  options.headers = params.headers;
  const proxyURL = (0, _proxyFromEnv.getProxyForUrl)(params.url);

  if (proxyURL) {
    if (params.url.startsWith('http:')) {
      const proxy = URL.parse(proxyURL);
      options = {
        path: parsedUrl.href,
        host: proxy.hostname,
        port: proxy.port
      };
    } else {
      const parsedProxyURL = URL.parse(proxyURL);
      parsedProxyURL.secureProxy = parsedProxyURL.protocol === 'https:';
      options.agent = new ProxyAgent(parsedProxyURL);
      options.rejectUnauthorized = false;
    }
  }

  const requestCallback = res => {
    const statusCode = res.statusCode || 0;
    if (statusCode >= 300 && statusCode < 400 && res.headers.location) httpRequest({ ...params,
      url: res.headers.location
    }, onResponse, onError);else onResponse(res);
  };

  const request = options.protocol === 'https:' ? _https.default.request(options, requestCallback) : _http.default.request(options, requestCallback);
  request.on('error', onError);

  if (params.timeout !== undefined) {
    const rejectOnTimeout = () => {
      onError(new Error(`Request to ${params.url} timed out after ${params.timeout}ms`));
      request.abort();
    };

    if (params.timeout <= 0) {
      rejectOnTimeout();
      return;
    }

    request.setTimeout(params.timeout, rejectOnTimeout);
  }

  request.end(params.data);
}

function fetchData(params, onError) {
  return new Promise((resolve, reject) => {
    httpRequest(params, async response => {
      if (response.statusCode !== 200) {
        const error = onError ? await onError(params, response) : new Error(`fetch failed: server returned code ${response.statusCode}. URL: ${params.url}`);
        reject(error);
        return;
      }

      let body = '';
      response.on('data', chunk => body += chunk);
      response.on('error', error => reject(error));
      response.on('end', () => resolve(body));
    }, reject);
  });
}

function downloadFile(url, destinationPath, options = {}) {
  const {
    progressCallback,
    log = () => {}
  } = options;
  log(`running download:`);
  log(`-- from url: ${url}`);
  log(`-- to location: ${destinationPath}`);

  let fulfill = ({
    error
  }) => {};

  let downloadedBytes = 0;
  let totalBytes = 0;
  const promise = new Promise(x => {
    fulfill = x;
  });
  httpRequest({
    url,
    headers: options.userAgent ? {
      'User-Agent': options.userAgent
    } : undefined
  }, response => {
    log(`-- response status code: ${response.statusCode}`);

    if (response.statusCode !== 200) {
      const error = new Error(`Download failed: server returned code ${response.statusCode}. URL: ${url}`); // consume response data to free up memory

      response.resume();
      fulfill({
        error
      });
      return;
    }

    const file = _fs.default.createWriteStream(destinationPath);

    file.on('finish', () => fulfill({
      error: null
    }));
    file.on('error', error => fulfill({
      error
    }));
    response.pipe(file);
    totalBytes = parseInt(response.headers['content-length'] || '0', 10);
    log(`-- total bytes: ${totalBytes}`);
    if (progressCallback) response.on('data', onData);
  }, error => fulfill({
    error
  }));
  return promise;

  function onData(chunk) {
    downloadedBytes += chunk.length;
    progressCallback(downloadedBytes, totalBytes);
  }
}

async function download(url, destination, options = {}) {
  const {
    progressBarName = 'file',
    retryCount = 3,
    log = () => {},
    userAgent
  } = options;

  for (let attempt = 1; attempt <= retryCount; ++attempt) {
    log(`downloading ${progressBarName} - attempt #${attempt}`);
    const {
      error
    } = await downloadFile(url, destination, {
      progressCallback: getDownloadProgress(progressBarName),
      log,
      userAgent
    });

    if (!error) {
      log(`SUCCESS downloading ${progressBarName}`);
      break;
    }

    const errorMessage = (error === null || error === void 0 ? void 0 : error.message) || '';
    log(`attempt #${attempt} - ERROR: ${errorMessage}`);

    if (attempt < retryCount && (errorMessage.includes('ECONNRESET') || errorMessage.includes('ETIMEDOUT'))) {
      // Maximum default delay is 3rd retry: 1337.5ms
      const millis = Math.random() * 200 + 250 * Math.pow(1.5, attempt);
      log(`sleeping ${millis}ms before retry...`);
      await new Promise(c => setTimeout(c, millis));
    } else {
      throw error;
    }
  }
}

function getDownloadProgress(progressBarName) {
  let progressBar;
  let lastDownloadedBytes = 0;
  return (downloadedBytes, totalBytes) => {
    if (!process.stderr.isTTY) return;

    if (!progressBar) {
      progressBar = new _progress.default(`Downloading ${progressBarName} - ${toMegabytes(totalBytes)} [:bar] :percent :etas `, {
        complete: '=',
        incomplete: ' ',
        width: 20,
        total: totalBytes
      });
    }

    const delta = downloadedBytes - lastDownloadedBytes;
    lastDownloadedBytes = downloadedBytes;
    progressBar.tick(delta);
  };
}

function toMegabytes(bytes) {
  const mb = bytes / 1024 / 1024;
  return `${Math.round(mb * 10) / 10} Mb`;
}

function spawnAsync(cmd, args, options = {}) {
  const process = (0, _child_process.spawn)(cmd, args, Object.assign({
    windowsHide: true
  }, options));
  return new Promise(resolve => {
    let stdout = '';
    let stderr = '';
    if (process.stdout) process.stdout.on('data', data => stdout += data);
    if (process.stderr) process.stderr.on('data', data => stderr += data);
    process.on('close', code => resolve({
      stdout,
      stderr,
      code
    }));
    process.on('error', error => resolve({
      stdout,
      stderr,
      code: 0,
      error
    }));
  });
} // See https://joel.tools/microtasks/


function makeWaitForNextTask() {
  // As of Mar 2021, Electron v12 doesn't create new task with `setImmediate` despite
  // using Node 14 internally, so we fallback to `setTimeout(0)` instead.
  // @see https://github.com/electron/electron/issues/28261
  if (process.versions.electron) return callback => setTimeout(callback, 0);
  if (parseInt(process.versions.node, 10) >= 11) return setImmediate; // Unlike Node 11, Node 10 and less have a bug with Task and MicroTask execution order:
  // - https://github.com/nodejs/node/issues/22257
  //
  // So we can't simply run setImmediate to dispatch code in a following task.
  // However, we can run setImmediate from-inside setImmediate to make sure we're getting
  // in the following task.

  let spinning = false;
  const callbacks = [];

  const loop = () => {
    const callback = callbacks.shift();

    if (!callback) {
      spinning = false;
      return;
    }

    setImmediate(loop); // Make sure to call callback() as the last thing since it's
    // untrusted code that might throw.

    callback();
  };

  return callback => {
    callbacks.push(callback);

    if (!spinning) {
      spinning = true;
      setImmediate(loop);
    }
  };
}

function assert(value, message) {
  if (!value) throw new Error(message || 'Assertion error');
}

function debugAssert(value, message) {
  if (isUnderTest() && !value) throw new Error(message);
}

function isString(obj) {
  return typeof obj === 'string' || obj instanceof String;
}

function isRegExp(obj) {
  return obj instanceof RegExp || Object.prototype.toString.call(obj) === '[object RegExp]';
}

function isObject(obj) {
  return typeof obj === 'object' && obj !== null;
}

function isError(obj) {
  return obj instanceof Error || obj && obj.__proto__ && obj.__proto__.name === 'Error';
}

const debugEnv = getFromENV('PWDEBUG') || '';

function debugMode() {
  if (debugEnv === 'console') return 'console';
  if (debugEnv === '0' || debugEnv === 'false') return '';
  return debugEnv ? 'inspector' : '';
}

let _isUnderTest = false;

function setUnderTest() {
  _isUnderTest = true;
}

function isUnderTest() {
  return _isUnderTest;
}

function getFromENV(name) {
  let value = process.env[name];
  value = value === undefined ? process.env[`npm_config_${name.toLowerCase()}`] : value;
  value = value === undefined ? process.env[`npm_package_config_${name.toLowerCase()}`] : value;
  return value;
}

function getAsBooleanFromENV(name) {
  const value = getFromENV(name);
  return !!value && value !== 'false' && value !== '0';
}

async function mkdirIfNeeded(filePath) {
  // This will harmlessly throw on windows if the dirname is the root directory.
  await _fs.default.promises.mkdir(_path.default.dirname(filePath), {
    recursive: true
  }).catch(() => {});
}

function headersObjectToArray(headers, separator, setCookieSeparator) {
  if (!setCookieSeparator) setCookieSeparator = separator;
  const result = [];

  for (const name in headers) {
    const values = headers[name];

    if (separator) {
      const sep = name.toLowerCase() === 'set-cookie' ? setCookieSeparator : separator;

      for (const value of values.split(sep)) result.push({
        name,
        value: value.trim()
      });
    } else {
      result.push({
        name,
        value: values
      });
    }
  }

  return result;
}

function headersArrayToObject(headers, lowerCase) {
  const result = {};

  for (const {
    name,
    value
  } of headers) result[lowerCase ? name.toLowerCase() : name] = value;

  return result;
}

function monotonicTime() {
  const [seconds, nanoseconds] = process.hrtime();
  return seconds * 1000 + (nanoseconds / 1000 | 0) / 1000;
}

function objectToArray(map) {
  if (!map) return undefined;
  const result = [];

  for (const [name, value] of Object.entries(map)) result.push({
    name,
    value: String(value)
  });

  return result;
}

function arrayToObject(array) {
  if (!array) return undefined;
  const result = {};

  for (const {
    name,
    value
  } of array) result[name] = value;

  return result;
}

function calculateSha1(buffer) {
  const hash = crypto.createHash('sha1');
  hash.update(buffer);
  return hash.digest('hex');
}

function createGuid() {
  return crypto.randomBytes(16).toString('hex');
}

async function removeFolders(dirs) {
  return await Promise.all(dirs.map(dir => {
    return new Promise(fulfill => {
      (0, _rimraf.default)(dir, {
        maxBusyTries: 10
      }, error => {
        fulfill(error !== null && error !== void 0 ? error : undefined);
      });
    });
  }));
}

function canAccessFile(file) {
  if (!file) return false;

  try {
    _fs.default.accessSync(file);

    return true;
  } catch (e) {
    return false;
  }
}

let cachedUserAgent;

function getUserAgent() {
  if (cachedUserAgent) return cachedUserAgent;

  try {
    cachedUserAgent = determineUserAgent();
  } catch (e) {
    cachedUserAgent = 'Playwright/unknown';
  }

  return cachedUserAgent;
}

function determineUserAgent() {
  let osIdentifier = 'unknown';
  let osVersion = 'unknown';

  if (process.platform === 'win32') {
    const version = _os.default.release().split('.');

    osIdentifier = 'windows';
    osVersion = `${version[0]}.${version[1]}`;
  } else if (process.platform === 'darwin') {
    const version = (0, _child_process.execSync)('sw_vers -productVersion', {
      stdio: ['ignore', 'pipe', 'ignore']
    }).toString().trim().split('.');
    osIdentifier = 'macOS';
    osVersion = `${version[0]}.${version[1]}`;
  } else if (process.platform === 'linux') {
    try {
      // List of /etc/os-release values for different distributions could be
      // found here: https://gist.github.com/aslushnikov/8ceddb8288e4cf9db3039c02e0f4fb75
      const osReleaseText = _fs.default.readFileSync('/etc/os-release', 'utf8');

      const fields = (0, _ubuntuVersion.parseOSReleaseText)(osReleaseText);
      osIdentifier = fields.get('id') || 'unknown';
      osVersion = fields.get('version_id') || 'unknown';
    } catch (e) {
      // Linux distribution without /etc/os-release.
      // Default to linux/unknown.
      osIdentifier = 'linux';
    }
  }

  let langName = 'unknown';
  let langVersion = 'unknown';

  if (!process.env.PW_LANG_NAME) {
    langName = 'node';
    langVersion = process.version.substring(1).split('.').slice(0, 2).join('.');
  } else if (['node', 'python', 'java', 'csharp'].includes(process.env.PW_LANG_NAME)) {
    var _process$env$PW_LANG_;

    langName = process.env.PW_LANG_NAME;
    langVersion = (_process$env$PW_LANG_ = process.env.PW_LANG_NAME_VERSION) !== null && _process$env$PW_LANG_ !== void 0 ? _process$env$PW_LANG_ : 'unknown';
  }

  return `Playwright/${getPlaywrightVersion()} (${_os.default.arch()}; ${osIdentifier} ${osVersion}) ${langName}/${langVersion}`;
}

function getPlaywrightVersion(majorMinorOnly = false) {
  const packageJson = require('./../../package.json');

  return majorMinorOnly ? packageJson.version.split('.').slice(0, 2).join('.') : packageJson.version;
}

function constructURLBasedOnBaseURL(baseURL, givenURL) {
  try {
    return new URL.URL(givenURL, baseURL).toString();
  } catch (e) {
    return givenURL;
  }
}

const hostPlatform = (() => {
  const platform = _os.default.platform();

  if (platform === 'darwin') {
    const ver = _os.default.release().split('.').map(a => parseInt(a, 10));

    let macVersion = '';

    if (ver[0] < 18) {
      // Everything before 10.14 is considered 10.13.
      macVersion = 'mac10.13';
    } else if (ver[0] === 18) {
      macVersion = 'mac10.14';
    } else if (ver[0] === 19) {
      macVersion = 'mac10.15';
    } else {
      // ver[0] >= 20
      const LAST_STABLE_MAC_MAJOR_VERSION = 12; // Best-effort support for MacOS beta versions.

      macVersion = 'mac' + Math.min(ver[0] - 9, LAST_STABLE_MAC_MAJOR_VERSION); // BigSur is the first version that might run on Apple Silicon.

      if (_os.default.cpus().some(cpu => cpu.model.includes('Apple'))) macVersion += '-arm64';
    }

    return macVersion;
  }

  if (platform === 'linux') {
    const ubuntuVersion = (0, _ubuntuVersion.getUbuntuVersionSync)();
    const archSuffix = _os.default.arch() === 'arm64' ? '-arm64' : '';
    if (parseInt(ubuntuVersion, 10) <= 19) return 'ubuntu18.04' + archSuffix;
    return 'ubuntu20.04' + archSuffix;
  }

  if (platform === 'win32') return 'win64';
  return platform;
})();

exports.hostPlatform = hostPlatform;

function wrapInASCIIBox(text, padding = 0) {
  const lines = text.split('\n');
  const maxLength = Math.max(...lines.map(line => line.length));
  return ['╔' + '═'.repeat(maxLength + padding * 2) + '╗', ...lines.map(line => '║' + ' '.repeat(padding) + line + ' '.repeat(maxLength - line.length + padding) + '║'), '╚' + '═'.repeat(maxLength + padding * 2) + '╝'].join('\n');
}

function isFilePayload(value) {
  return typeof value === 'object' && value['name'] && value['mimeType'] && value['buffer'];
}

function streamToString(stream) {
  return new Promise((resolve, reject) => {
    const chunks = [];
    stream.on('data', chunk => chunks.push(Buffer.from(chunk)));
    stream.on('error', reject);
    stream.on('end', () => resolve(Buffer.concat(chunks).toString('utf8')));
  });
}

async function transformCommandsForRoot(commands) {
  const isRoot = process.getuid() === 0;
  if (isRoot) return {
    command: 'sh',
    args: ['-c', `${commands.join('&& ')}`],
    elevatedPermissions: false
  };
  const sudoExists = await spawnAsync('which', ['sudo']);
  if (sudoExists.code === 0) return {
    command: 'sudo',
    args: ['--', 'sh', '-c', `${commands.join('&& ')}`],
    elevatedPermissions: true
  };
  return {
    command: 'su',
    args: ['root', '-c', `${commands.join('&& ')}`],
    elevatedPermissions: true
  };
}

class SigIntWatcher {
  constructor() {
    this._hadSignal = false;
    this._sigintPromise = void 0;
    this._sigintHandler = void 0;
    let sigintCallback;
    this._sigintPromise = new Promise(f => sigintCallback = f);

    this._sigintHandler = () => {
      // We remove the handler so that second Ctrl+C immediately kills the runner
      // via the default sigint handler. This is handy in the case where our shutdown
      // takes a lot of time or is buggy.
      //
      // When running through NPM we might get multiple SIGINT signals
      // for a single Ctrl+C - this is an NPM bug present since at least NPM v6.
      // https://github.com/npm/cli/issues/1591
      // https://github.com/npm/cli/issues/2124
      //
      // Therefore, removing the handler too soon will just kill the process
      // with default handler without printing the results.
      // We work around this by giving NPM 1000ms to send us duplicate signals.
      // The side effect is that slow shutdown or bug in our runner will force
      // the user to hit Ctrl+C again after at least a second.
      setTimeout(() => process.off('SIGINT', this._sigintHandler), 1000);
      this._hadSignal = true;
      sigintCallback();
    };

    process.on('SIGINT', this._sigintHandler);
  }

  promise() {
    return this._sigintPromise;
  }

  hadSignal() {
    return this._hadSignal;
  }

  disarm() {
    process.off('SIGINT', this._sigintHandler);
  }

}

exports.SigIntWatcher = SigIntWatcher;