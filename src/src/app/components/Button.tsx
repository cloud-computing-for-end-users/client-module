import * as React from "react";
import { ipcRenderer } from "electron";

export class Button extends React.Component<{}> {
  handleClick() {
      console.log('The link was clicked');
  }

  public render(): React.ReactNode {
    return <button onClick={this.handleClick}>Click me!</button>;
  }
}    