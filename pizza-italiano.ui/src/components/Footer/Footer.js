function Footer() {
    const year = new Date().getFullYear();
    
    return(
        <div className="text-center m-3 text-primary">
            &copy; Kamil Wojtasiński {year}
        </div>
    );
}

export default Footer;