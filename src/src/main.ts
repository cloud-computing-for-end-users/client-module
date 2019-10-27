import { WindowManager } from "./main/WindowManager";
import { IPCMainManager } from "./main/IPCMainManager";
import { CGIConnectionManager } from "./main/CGIConnectionManager";
import { ElectronManager } from "./main/ElectronManager";

let CGIConnectionHandler: CGIConnectionManager = new CGIConnectionManager(); 
let WindowHandler: WindowManager = new WindowManager(CGIConnectionHandler);
new IPCMainManager(WindowHandler, CGIConnectionHandler);
new ElectronManager(WindowHandler);
