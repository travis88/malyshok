﻿var ImportInfo = React.createClass({
    getInitialState: function () {
        return { data: [] };
    },
    loadFromServer: function () {
        var xhr = new XMLHttpRequest();
        xhr.open('get', this.props.url, true);
        xhr.onload = function () {
            var data = JSON.parse(xhr.responseText);
            this.setState({ data: data });
        }.bind(this);
        xhr.send();
    },
    componentDidMount: function () {
        this.loadFromServer();
        window.setInterval(this.loadFromServer, this.props.pollInterval)
    },
    render: function () {
        return (
            <div className="import-info">
                {this.state.data.Day}.{this.state.data.Month}.{this.state.data.Year} | {this.state.data.Hour}:{this.state.data.Minute}:{this.state.data.Second}
          </div>
        );
    }
});
ReactDOM.render(
    <ImportInfo url="/admin/import/importprocessed" pollInterval={1000} />,
    document.getElementById('content')
); 