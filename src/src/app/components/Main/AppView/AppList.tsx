import * as React from "react";
import {AppListItem} from "./AppListItem";

interface IState { }

interface IProps { }

export class AppList extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
  }

  public render(): React.ReactNode {
    return (
      <ul className="list-group mt-3">
        <AppListItem />
      </ul>
    );
  }
}
