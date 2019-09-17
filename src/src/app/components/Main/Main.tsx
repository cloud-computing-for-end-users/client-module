import * as React from "react";
import * as $ from "jquery";
import {Navigation} from "./Navigation/Navigation";
import {AppView} from "./AppView/AppView";
import {FileView} from "./FileView/FileView";
import {WindowControls} from "../Shared/WindowControls";

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

  disableDragging(): void {
    $('img').prop('draggable', false);
    $('a').prop('draggable', false);
  }

  componentDidUpdate(): void {
    this.disableDragging();
  }

  componentDidMount(): void {
    this.disableDragging();
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

    return ( [ 
        <WindowControls key="WindowControls" />,
        <Navigation key="Navigation" active={this.state.content} onViewChange={this.handleViewChange} />,
        <div key="MainView" className="container">
          {view}    
        </div>
      ]
    );
  }
}