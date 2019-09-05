import * as React from "react";
import {AppList} from "./AppList";

interface IState { }

interface IProps { }

export class AppView extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
  }

  public render(): React.ReactNode {
    return (
      <div className="row">
        <AppList />
      </div>
    );
  }
}
