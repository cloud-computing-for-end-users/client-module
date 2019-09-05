import * as React from "react";
import {Navigation} from "./Navigation/Navigation";
import {AppView} from "./AppView/AppView";
import {FileView} from "./FileView/FileView";

interface IState {
  content: ContentType
}

interface IProps { }

export enum ContentType {
  AppView = "Apps",
  FileView = "Files"
}

export class Main extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
    this.handleViewChange = this.handleViewChange.bind(this);
    this.state = { 
      content: ContentType.AppView // default Apps tab
    };
  }

  public handleViewChange(content: ContentType): void {
    this.setState({content});
  }  

  public render(): React.ReactNode {
    var view;
    switch(this.state.content) {
      case ContentType.AppView: view = <AppView />; break;
      case ContentType.FileView: view = <FileView />; break;
    }

    return (
      <div>  
        <Navigation active={this.state.content} onViewChange={this.handleViewChange} />
        <div className="container">
          {view}    
        </div>
      </div>
    );
  }
}