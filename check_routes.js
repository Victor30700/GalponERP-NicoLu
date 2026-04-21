const fs = require('fs');
const path = require('path');

const controllersDir = path.join('GalponERP.Api', 'Controllers');
const frontendDir = path.join('frontend', 'src');

function getFiles(dir, ext) {
    let results = [];
    const list = fs.readdirSync(dir);
    list.forEach(file => {
        file = path.join(dir, file);
        const stat = fs.statSync(file);
        if (stat && stat.isDirectory()) {
            results = results.concat(getFiles(file, ext));
        } else if (file.endsWith(ext)) {
            results.push(file);
        }
    });
    return results;
}

const csharpFiles = getFiles(controllersDir, '.cs');
const tsFiles = getFiles(frontendDir, '.ts').concat(getFiles(frontendDir, '.tsx'));

let allRoutes = [];

csharpFiles.forEach(file => {
    const content = fs.readFileSync(file, 'utf-8');
    const controllerNameMatch = content.match(/class (\w+)Controller/);
    if (!controllerNameMatch) return;
    const controllerName = controllerNameMatch[1];
    
    const routeMatch = content.match(/\[Route\(\"([^\"]+)\"\)\]/);
    let baseRoute = routeMatch ? routeMatch[1].replace('[controller]', controllerName) : `api/${controllerName}`;
    
    // Handles [HttpGet], [HttpPost("path")], etc.
    const methodRegex = /\[Http(Get|Post|Put|Delete|Patch)(?:\(\"([^\"]+)\"\))?\]/g;
    let match;
    while ((match = methodRegex.exec(content)) !== null) {
        const method = match[1].toUpperCase();
        const subRoute = match[2] ? `/${match[2]}` : '';
        const fullRoute = `/${baseRoute}${subRoute}`.replace(/\/\/+/g, '/');
        allRoutes.push({ method, route: fullRoute, file: path.basename(file) });
    }
});

const tsContents = tsFiles.map(f => fs.readFileSync(f, 'utf-8')).join('\n');

const missing = [];
allRoutes.forEach(r => {
    // create a regex to match the route, ignoring path params like {id}
    const searchPattern = r.route.replace(/\{[^\}]+\}/g, '[^/`\'"\\?]+');
    const regex = new RegExp(searchPattern, 'i');
    if (!regex.test(tsContents)) {
        missing.push(`${r.method} ${r.route} (from ${r.file})`);
    }
});

console.log('--- MISSING ENDPOINTS IN FRONTEND ---');
missing.forEach(m => console.log(m));