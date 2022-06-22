import styles from './Header.module.css';

function Header(props) {
    return (
            <header className={`${styles.header}`}>
                <div 
                    className={styles.headerImage}>
                </div>
                <div>
                    <div>
                        <div className='container-fluid'>
                            <div className='d-flex justify-content-center align-items-center'>
                                <div className={styles.backgroundName}>
                                    <h3 className={styles.headerName}>
                                        PizzaItaliano App
                                    </h3>
                                </div>
                                <div className='ms-2 text-center'>
                                    {props.children}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </header>
    );
}

export default Header;