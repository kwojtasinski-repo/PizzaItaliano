import styles from "./Popup.module.css";
import PropTypes from "prop-types";
import LoadingButton from "../UI/LoadingButton/LoadingButton";

function Popup(props) {
    switch (props.popupType) { 
        case "send":
            return <PopupSend {...props} />;
        default :
            return <PopupDefault {...props} />;
    }
}

function PopupDefault(props) {
    return (
        <div className={styles.popupBox}>
            <div className={styles.box}>
                {props.content}
                <button className={`btn btn-${props.type}`} onClick = {props.handleConfirm} >{props.textConfirm ? props.textConfirm : "OK"}</button>
                <button className="btn btn-secondary ms-2" onClick = {props.handleClose} >Cancel</button>
            </div>
        </div>
    )
}

function PopupSend(props) {
    return (
        <div className={styles.popupBox}>
            <div className={styles.box}>
                {props.content}
                <LoadingButton className={`btn btn-${props.type}`} 
                               onClick = {props.handleConfirm}
                               loading = {props.loading} >
                    {props.textConfirm ? props.textConfirm : "Apply"}
                </LoadingButton>
                <button className="btn btn-secondary ms-2" onClick = {props.handleClose} >Cancel</button>
            </div>
        </div>
    )
}

export default Popup;

export const Type = {
    info: "primary",
    success: "success",
    warning: "warning",
    alert: "danger",
};

Popup.propTypes = {
    type: PropTypes.oneOf(Object.values(Type))
};

Popup.defaultProps = {
    type: Type.info
}