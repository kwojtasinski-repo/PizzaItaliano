import styles from './Items.module.css'
import Item from './Item/Item';

function Items(props) {
    return (
        <div>
            <h1 className={styles.title}>Offers:</h1>
            <div className='row me-2'>
                {props.items.map(i => 
                            <Item key={i.id}
                                  {...i} />
                )}
            </div>
            
        </div>
    );
}

export default Items;