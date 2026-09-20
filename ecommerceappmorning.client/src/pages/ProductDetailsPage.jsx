import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Box, Button, Container, Grid, Typography } from "@mui/material";

import { addToCart } from "../services/cartService";

import api from "../services/api";
import { useAuth } from "../context/AuthContext";

function ProductDetailsPage() {
  const { id } = useParams();
  const {user}=useAuth();
  const userId=user?.userId;

  const [product, setProduct] = useState(null);

  const [quantity, setQuantity] = useState(1);

  const handleAddToCart = async () => {
    try {
      await addToCart(userId, product.id, quantity);

      alert("Product added to cart!");
    } catch (error) {
      console.error(error);
    }
  };

  useEffect(() => {
    const getProduct = async () => {
      try {
        const response = await api.get(`/products/${id}`);

        setProduct(response.data);
      } catch (error) {
        console.error(error);
      }
    };

    getProduct();
  }, [id]);

  if (!product) {
    return (
      <Container sx={{ py: 5 }}>
        <Typography>Loading...</Typography>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ py: 6 }}>
      <Grid container spacing={6}>
        <Grid size={{ xs: 12, md: 6 }}>
          <Box
            component="img"
            src={product.imageUrl}
            alt={product.name}
            sx={{
              width: "100%",
              borderRadius: 3,
            }}
          />
        </Grid>

        <Grid size={{ xs: 12, md: 6 }}>
          <Typography variant="h3" fontWeight="bold">
            {product.name}
          </Typography>

          <Typography color="text.secondary" sx={{ mt: 2 }}>
            {product.category?.name}
          </Typography>

          <Typography variant="h4" fontWeight="bold" sx={{ mt: 3 }}>
            ${product.price}
          </Typography>

          <Typography sx={{ mt: 3 }}>{product.description}</Typography>

          <Typography sx={{ mt: 3 }}>Stock: {product.stock}</Typography>

          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: 2,
              mt: 3,
            }}
          >
            <Button
              variant="outlined"
              onClick={() => setQuantity((q) => Math.max(1, q - 1))}
            >
              -
            </Button>

            <Typography>{quantity}</Typography>

            <Button
              variant="outlined"
              onClick={() => setQuantity((q) => Math.min(product.stock, q + 1))}
            >
              +
            </Button>
          </Box>

          <Button
            variant="contained"
            size="large"
            sx={{ mt: 3 }}
            disabled={product.stock === 0}
            onClick={handleAddToCart}
          >
            Add To Cart
          </Button>
        </Grid>
      </Grid>
    </Container>
  );
}

export default ProductDetailsPage;
