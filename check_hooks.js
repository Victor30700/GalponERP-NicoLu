const fs = require('fs');
const path = require('path');

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

const hooksDir = path.join('frontend', 'src', 'hooks');
const appDir = path.join('frontend', 'src', 'app');
const compDir = path.join('frontend', 'src', 'components');

const hookFiles = getFiles(hooksDir, '.ts');
const appFiles = getFiles(appDir, '.tsx').concat(getFiles(appDir, '.ts'));
const compFiles = getFiles(compDir, '.tsx').concat(getFiles(compDir, '.ts'));
const allUiFiles = appFiles.concat(compFiles);

const uiContents = allUiFiles.map(f => fs.readFileSync(f, 'utf-8')).join('\n');

const exportedFunctions = [];

hookFiles.forEach(file => {
    const content = fs.readFileSync(file, 'utf-8');
    const regex = /export function (use\w+)/g;
    let match;
    while ((match = regex.exec(content)) !== null) {
        exportedFunctions.push({ name: match[1], file: path.basename(file) });
    }
});

const unusedHooks = [];
exportedFunctions.forEach(fn => {
    // Check if hook is used in app or components files
    const regex = new RegExp(`\\b${fn.name}\\b`);
    if (!regex.test(uiContents)) {
        unusedHooks.push(`${fn.name} (from ${fn.file})`);
    }
});

console.log('--- UNUSED HOOKS IN FRONTEND ---');
unusedHooks.forEach(m => console.log(m));