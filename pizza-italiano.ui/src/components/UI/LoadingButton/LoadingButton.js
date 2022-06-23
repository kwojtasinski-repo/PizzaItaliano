export default function LoadingButton(props) {
    const buttonProps = {...props};
    delete buttonProps.loading;

    return (props.loading ?
        (
            <button style={buttonProps.style ? buttonProps.style : null}
                className={buttonProps.className ? buttonProps.className : `btn btn-success`} disabled >
                <span className="spinner-border spinner-border-sm" role="status"></span>
                <span className="sr-only">Loading...</span>
            </button>
        )
        : <button {...buttonProps} className={buttonProps.className ? buttonProps.className : `btn btn-success`}>{props.children}</button>
    )
}