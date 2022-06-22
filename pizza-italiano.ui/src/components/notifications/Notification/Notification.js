import React from "react";
import PropTypes from "prop-types";
import { ReactComponent as Times } from "./times.svg";
import styles from "./Notification.module.css";
import createContainer from "../container/createContainer";
import { createPortal } from "react-dom";

const container = createContainer();
let timeToDelete = 3000;

export default function Notification({
    color = Color.info,
    autoClose = false,
    children,
  }) {
    const [isClosing, setIsClosing] = React.useState(false);

    React.useEffect(() => {
        if (autoClose) {
            const timeoutId = setTimeout(() => setIsClosing(true), timeToDelete);

            return () => {
            clearTimeout(timeoutId);
            };
        }
    }, [autoClose]);
    
    

    return createPortal(
        <div className={`${styles.container} ${isClosing ? styles.shrink : ''}`}>
          <div
            className={`${styles.notification}
              ${styles[color]} ${!isClosing ? styles.slideIn : '' } ${isClosing ? styles.slideOut : ''}`} >
            {children}
            <button
              onClick={() => setIsClosing(true)}
              className={styles.closeButton}
            >
              <Times height={16} />
            </button>
          </div>
        </div>,
        container
      )
}

export const Color = {
  info: "info",
  success: "success",
  warning: "warning",
  error: "error",
};

Notification.propTypes = {
  notificationType: PropTypes.oneOf(Object.keys(Color)),
  children: PropTypes.string,
};