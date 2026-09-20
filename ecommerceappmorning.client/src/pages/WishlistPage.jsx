import { useEffect, useState } from 'react';
import {
  Box,
  Card,
  CardContent,
  CardMedia,
  Container,
  Grid,
  IconButton,
  Skeleton,
  Typography,
  Button
} from '@mui/material';
import FavoriteIcon from '@mui/icons-material/Favorite';
import { useNavigate, Link } from 'react-router-dom';

import { wishlistService } from '../services/wishlistService';

export default function WishlistPage() {
  const navigate = useNavigate();
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchWishlist = async () => {
    try {
      setLoading(true);
      const data = await wishlistService.getWishlist();
      setItems(data);
    } catch (err) {
      console.error('Wishlist yüklenirken hata oluştu:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchWishlist();
  }, []);

  const handleRemove = async (e, productId) => {
    e.stopPropagation(); 

    try {
      await wishlistService.toggleWishlist(productId);
      setItems((prevItems) =>
        prevItems.filter(
          (item) => (item.productId || item.id) !== productId
        )
      );
    } catch (err) {
      console.error('Silinirken hata oluştu:', err);
    }
  };

  return (
    <Container maxWidth="xl" sx={{ py: 5 }}>
      <Typography variant="h3" fontWeight="bold" sx={{ mb: 4 }}>
        İstek Listem (Wishlist)
      </Typography>

      {loading ? (
        <Grid container spacing={3}>
          {Array.from({ length: 4 }).map((_, index) => (
            <Grid key={index} item xs={12} sm={6} md={4} lg={3}>
              <Skeleton variant="rectangular" height={250} sx={{ borderRadius: 2 }} />
              <Skeleton height={40} sx={{ mt: 1 }} />
              <Skeleton width="60%" />
            </Grid>
          ))}
        </Grid>
      ) : items.length === 0 ? (
        <Box sx={{ textAlign: 'center', py: 10 }}>
          <Typography variant="h5" sx={{ mb: 1 }}>
            İstek listenizde ürün bulunmamaktadır ❤️
          </Typography>
          <Typography color="text.secondary" sx={{ mb: 3 }}>
            Beğendiğiniz ürünleri buraya ekleyebilirsiniz.
          </Typography>
          <Button component={Link} to="/products" variant="contained">
            Ürünlere Göz At
          </Button>
        </Box>
      ) : (
        <Grid container spacing={3}>
          {items.map((item) => {
           
            const productId = item.productId || item.id;
            const title = item.name || item.title || item.productName;
            const price = item.price;
            const imageUrl = item.imageUrl;

            return (
              <Grid key={productId} item xs={12} sm={6} md={4} lg={3}>
                <Card
                  onClick={() => navigate(`/products/${productId}`)}
                  sx={{
                    height: '100%',
                    cursor: 'pointer',
                    borderRadius: 3,
                    overflow: 'hidden',
                    position: 'relative',
                    transition: '0.3s',
                    '&:hover': {
                      transform: 'translateY(-6px)',
                      boxShadow: 6,
                    },
                  }}
                >
                 
                  <IconButton
                    onClick={(e) => handleRemove(e, productId)}
                    sx={{
                      position: 'absolute',
                      top: 10,
                      right: 10,
                      backgroundColor: 'rgba(255, 255, 255, 0.85)',
                      backdropFilter: 'blur(4px)',
                      '&:hover': {
                        backgroundColor: '#ffffff',
                      },
                      zIndex: 2,
                    }}
                  >
                    <FavoriteIcon sx={{ color: '#e11d48' }} />
                  </IconButton>

                  <CardMedia
                    component="img"
                    height="240"
                    image={imageUrl}
                    alt={title}
                  />

                  <CardContent>
                    <Typography variant="h6" fontWeight="bold" noWrap>
                      {title}
                    </Typography>

                    {item.categoryName && (
                      <Typography variant="body2" color="text.secondary">
                        {item.categoryName}
                      </Typography>
                    )}

                    <Typography variant="h6" fontWeight="bold" sx={{ mt: 2 }}>
                      ${price ? Number(price).toFixed(2) : '0.00'}
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
            );
          })}
        </Grid>
      )}
    </Container>
  );
}