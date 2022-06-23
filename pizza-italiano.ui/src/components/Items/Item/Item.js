import { addItemToCart } from '../../Cart/Cart';
import styles from './Item.module.css'

function Item(props) {
    const onClickHandler = (item) => {
        addItemToCart(item);
        window.location.reload();
    }

    return (
        <div className={`card ${styles.itemBorder} ms-2 mt-2`}>
            <div className="card-body">
               <div className="row">
                    <div className="col-4">
                        <div className="row">
                            <div className="col">
                                <h4>{props.name}</h4>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col mt-5">
                                <p>Cost:</p>
                                <h4>{props.cost} USD</h4>
                            </div>
                        </div>
                    </div>
                    <div>
                        <button className={`btn btn-primary mt-2 px-5 float-end`} style={{width: '14rem'}} onClick={onClickHandler.bind(this, props)} >
                            Add to cart
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Item;