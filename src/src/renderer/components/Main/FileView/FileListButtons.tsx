import * as React from "react";
import * as $ from "jquery";
import {BackendMethods} from "../../../renderer";
import { Utils } from "../../../../utils/Utils";
const { ipcRenderer } = require('electron');

interface IState {
  renameFileInput: string
  runningApps: any
}

interface IProps {
  loggedInAs: number
  getFileListItems: any
}

export class FileListButtons extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleOnClickDownload = this.handleOnClickDownload.bind(this);
    this.handleOnClickUpload = this.handleOnClickUpload.bind(this);
    this.handleOnClickRename = this.handleOnClickRename.bind(this);
    this.handleOnClickSend = this.handleOnClickSend.bind(this);
    this.handleRenameFileInputChange = this.handleRenameFileInputChange.bind(this);
    this.handleOnClickUpdateSendDropdown = this.handleOnClickUpdateSendDropdown.bind(this);

    this.state = {
      renameFileInput: "",
      runningApps: null
    }
  }

  componentWillUnmount() {
    ipcRenderer.removeAllListeners('reply-backend-method-' + BackendMethods.DownloadFile);
    ipcRenderer.removeAllListeners('reply-backend-method-' + BackendMethods.UploadFile);
    ipcRenderer.removeAllListeners('reply-backend-method-' + BackendMethods.RenameFile);
    ipcRenderer.removeAllListeners('reply-backend-method-' + BackendMethods.TellSlaveToFetchFile);
    ipcRenderer.removeAllListeners('replyGetRunningApps');
  }

  private handleOnClickDownload() {
    let fileName = this.getActiveFileName();
    if(fileName !== "") {
      ipcRenderer.send('call-backend-method', {
        method: BackendMethods.DownloadFile, 
        argument: {
          PrimaryKey: Utils.getLoggedInAs(this.props.loggedInAs),
          FileName: fileName
        }
      });
      ipcRenderer.on('reply-backend-method-' + BackendMethods.DownloadFile, (event, arg) => {
        console.log(arg + " - remove me (todo)");
        // todo check that arg is "Done (DownloadFile)" and show it in some way
      })
    } else {
      // todo nicer error message instead of alert
      alert("Choose a file - (todo)")
    }
  }

  private handleOnClickUpload() {
    const { dialog } = require('electron').remote
    dialog.showOpenDialog({
      properties: ['openFile']
    }).then(result => {
      if(result.filePaths.length === 1) {
        ipcRenderer.send('call-backend-method', {
          method: BackendMethods.UploadFile, 
          argument: {
            PrimaryKey: Utils.getLoggedInAs(this.props.loggedInAs),
            FileName:  result.filePaths[0]
          }
        });
        ipcRenderer.on('reply-backend-method-' + BackendMethods.UploadFile, (event, arg) => {
          if(arg === "Done (UploadFile)") {
            this.props.getFileListItems();
            // todo show successful upload in some way, updating list might be enough though
          }
        })
      } else {
        if(result.filePaths.length === 0){
          // OK
        } else {
          // todo figure what else could possibly happen?
          alert("Something weird happened; file paths length: " + result.filePaths.length)
        }
      }
    }).catch(err => {
      // todo figure out when this happens?
      console.log(err)
      alert(err)
    });
  }

  private handleRenameFileInputChange(event: any): void {
    this.setState({
      renameFileInput: event.target.value
    });
  }

  private handleOnClickRename() {
    let fileName = this.getActiveFileName();
    if(fileName !== "") {
      ipcRenderer.send('call-backend-method', {
        method: BackendMethods.RenameFile, 
        argument: {
          PrimaryKey: Utils.getLoggedInAs(this.props.loggedInAs),
          OldFileName: fileName,
          NewFileName: this.state.renameFileInput
        }
      });
      ipcRenderer.on('reply-backend-method-' + BackendMethods.RenameFile, (event, arg) => {
        console.log(arg + " - remove me (todo)");
        // todo check that arg is "Done (RenameFile)" and show it in some way
        this.props.getFileListItems();
      })
    } else {
      // todo nicer error message instead of alert
      alert("Choose a file - (todo)")
    }
  }

  private handleOnClickSend(event: any) {
    let fileName = this.getActiveFileName();
    if(fileName !== "") {
      let slaveKey = event.target.getAttribute('data-slavekey');
      if(slaveKey === "") {
        alert("Enter a new file name including extension");
      } else {
        ipcRenderer.send('call-backend-method', {
          method: BackendMethods.TellSlaveToFetchFile, 
          argument: {
            SlaveKey: slaveKey,
            FileName: fileName
          }
        });
        ipcRenderer.on('reply-backend-method-' + BackendMethods.TellSlaveToFetchFile, (event, arg) => {
          console.log(arg + " - remove me (todo)");
          // todo check that arg is "Sent (TellSlaveToFetchFile)" and show it in some way
        })
      }
    } else {
      // todo nicer error message instead of alert
      alert("Choose a file - (todo)")
    }
  }

  private handleOnClickUpdateSendDropdown(event: any) {
    ipcRenderer.send('getRunningApps', {});
    ipcRenderer.on('replyGetRunningApps', (event, arg) => {
      if(arg.length === 0) {
        this.setState({
          runningApps: <h6 className="dropdown-header">No running apps</h6>
        })
      } else {
        let runningApps: JSX.Element[] = [];
        arg.forEach((element: any) => {
          runningApps.push(<button key={element.slaveKey} className="dropdown-item" onClick={this.handleOnClickSend} data-slavekey={element.slaveKey}>{element.appName}</button>)
        });
        this.setState({runningApps})
      }
    })
  }

  private getActiveFileName() {
    let span = $("#file-list li.active div span");
    if(span.length === 1) { return span[0].innerText }
    else { return "" }
  }

  render(): React.ReactNode {    
    // todo change buttons to active / non-active if a file is selected
    // todo make the button layout prettier

    return (
      <div id="file-list-buttons">
        <button onClick={this.handleOnClickDownload} className="btn btn-outline-primary">Download file</button>
        <button onClick={this.handleOnClickUpload} className="btn btn-outline-primary">Upload new file</button>
        <div className="d-flex flex-column">
          <input value={this.state.renameFileInput} onChange={this.handleRenameFileInputChange} type="text" className="form-control" placeholder="New file name" />
          <button onClick={this.handleOnClickRename} className="btn btn-outline-primary">Rename file</button>
        </div>
        <button onMouseDown={this.handleOnClickUpdateSendDropdown} className="btn btn-outline-primary dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
          Send file to an application
        </button>
        <div className="dropdown-menu" aria-labelledby="dropdownMenuButton">
          {this.state.runningApps}
        </div>
      </div>
    );
  }
}
