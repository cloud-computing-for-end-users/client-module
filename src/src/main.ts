import { WindowManager } from "./main/WindowManager";
import { IPCMainManager } from "./main/IPCMainManager";
import { CGIConnectionManager } from "./main/CGIConnectionManager";
import { ElectronManager } from "./main/ElectronManager";

let WindowHandler: WindowManager = new WindowManager();
let CGIConnectionHandler: CGIConnectionManager = new CGIConnectionManager(); 
new IPCMainManager(WindowHandler, CGIConnectionHandler);
new ElectronManager(WindowHandler);
