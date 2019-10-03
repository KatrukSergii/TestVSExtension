'use strict';

import * as vscode from 'vscode';
import * as cp from 'child_process';
import * as rpc from 'vscode-jsonrpc';

export function activate(context: vscode.ExtensionContext) {
	let dotnnetExe = "dotnet";
	let procPath = "C:\\temp\\ConsoleAppJSONRPCTest\\ConsoleAppJSONRPCTest.exe";
	let childProcess = cp.spawn(procPath);
	
	// Use stdin and stdout for communication:
	let connection = rpc.createMessageConnection(
		new rpc.StreamMessageReader(childProcess.stdout),
		new rpc.StreamMessageWriter(childProcess.stdin));
 
	connection.listen();			
	const writeEmitter = new vscode.EventEmitter<string>();
	connection.sendRequest("ConfigureProject", ["ConfigOptions2"]).then((res) => 
	{
		writeEmitter.fire((res as string).toString());
	});
	
	connection.sendRequest("ConfigureProjectAuth", [new AuthOptions(AuthTypes.QS, "IntegratorKey")]).then((res) => 
	{
		writeEmitter.fire((res as string).toString());
	});
	
	context.		subscriptions.push(vscode.commands.registerCommand('extensionTerminalSample.create', () => {
		let line = '';
		const pty = {
			onDidWrite: writeEmitter.event,
			open: () => writeEmitter.fire('Type and press enter to echo the text\r\n\r\n'),
			close: () => {},
			handleInput: (data: string) => {
				if (data === '\r') { // Enter
					writeEmitter.fire(`\r\necho: "${colorText(line)}"\r\n\n`);
					line = '';
					return;
				}
				if (data === '\x7f') { // Backspace
					if (line.length === 0) {
						return;
					}
					line = line.substr(0, line.length - 1);
					// Move cursor backward
					writeEmitter.fire('\x1b[D');
					// Delete character
					writeEmitter.fire('\x1b[P');
					return;
				}
				line += data;
				writeEmitter.fire(data);
			}
		};
		const terminal = (<any>vscode.window).createTerminal({ name: `My Extension REPL`, pty });
		terminal.show();
	}));

}

const AuthTypes = {
    CodeGrant: 0,
    JWT: 1,
    QS: 2
}
class AuthOptions{
	AuthType: number;
	IntegratorKey: string;
	constructor(authType: number, integratorKey: string)
	{
		this.AuthType = authType;
		this.IntegratorKey = integratorKey;
	}
}

function outputResult(res: any)
{
	const writeEmitter = new vscode.EventEmitter<string>();
	writeEmitter.fire((res as number).toString());
}

function colorText(text: string): string {
	let output = '';
	let colorIndex = 1;
	for (let i = 0; i < text.length; i++) {
		const char = text.charAt(i);
		if (char === ' ' || char === '\r' || char === '\n') {
			output += char;
		} else {
			output += `\x1b[3${colorIndex++}m${text.charAt(i)}\x1b[0m`;
			if (colorIndex > 6) {
				colorIndex = 1;
			}
		}
	}
	return output;
}