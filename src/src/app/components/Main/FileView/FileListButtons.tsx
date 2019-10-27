import * as React from "react";
    
interface IState {}

interface IProps {
  loggedInAs: number
}

export class FileListButtons extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleOnClickDownload = this.handleOnClickDownload.bind(this);
    this.handleOnClickUpload = this.handleOnClickUpload.bind(this);
    this.handleOnClickRename = this.handleOnClickRename.bind(this);
    this.handleOnClickSend = this.handleOnClickSend.bind(this);
  }

  private handleOnClickDownload() {
    alert("Not implemented download");
  }

  private handleOnClickUpload() {
    alert("Not implemented upload");
  }

  private handleOnClickRename() {
    alert("Not implemented rename");
  }

  private handleOnClickSend() {
    const { BrowserWindow, remote } = require('electron')
    const url = require("url");
    const path = require("path");

    console.log(path.join(__dirname, "modal.html"));
    alert(path.join(__dirname, "modal.html"));
    /*
    var currentWindow: any = remote.getCurrentWindow();
    let modal = new BrowserWindow({ parent: currentWindow, modal: true, show: false })
    modal.loadURL(
      url.format({
          pathname: path.join(__dirname, "modal.html"),
          protocol: "file:",
          slashes: true
      })
    );
    
    child.loadURL('https://github.com')
    child.once('ready-to-show', () => {
      child.show()
    })

    alert("Not implemented send");
    */
  }

  public render(): React.ReactNode {    
    return (
      <div id="file-list-buttons">
        <button onClick={this.handleOnClickDownload} className="btn btn-outline-primary">Download file</button>
        <button onClick={this.handleOnClickUpload} className="btn btn-outline-primary">Upload new file</button>
        <button onClick={this.handleOnClickRename} className="btn btn-outline-primary">Rename file</button>
        <button onClick={this.handleOnClickSend} className="btn btn-outline-primary">Send file to an application</button>
      </div>
    );
  }
}
