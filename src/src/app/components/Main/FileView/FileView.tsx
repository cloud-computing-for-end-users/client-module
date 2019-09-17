import * as React from "react";

interface IState {
  message: string;
}

interface IProps { }

export class FileView extends React.Component<IProps, IState> {
  constructor(props: IProps) {
    super(props);
  }

  public render(): React.ReactNode {
    return (
      <div className="row">
        NOT IMPLEMENTED  
      </div>
    );
  }
}
